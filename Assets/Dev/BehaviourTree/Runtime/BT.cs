using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public class BTExecutor
    {
        private BTNRoot _rootNode;
        private BTNode _currentNode;
        private BTMain _tree;
        private BTServiceScheduler _serviceScheduler;
        private Stack<IBTDecoratorOwner> _decoOwnerStack;
        private Stack<IBTDecoratorOwner> _decoOwnerStackSwap;
        int countD = 0;

        
        public BTExecutor(BTMain treeMain)
        {
            this._rootNode = treeMain.root;
            this._currentNode = treeMain.root;
            this._tree = treeMain;

            treeMain.Init();

            _serviceScheduler = new();
            _decoOwnerStack = new(10);
            _decoOwnerStackSwap = new(10);

            foreach (var node in treeMain.nodes)
            {
                node.Init(treeMain);
            }
        }

        public void Update()
        {
            var result = EvaluateDecorator(ref _currentNode, false);
            _currentNode = EValuate(_currentNode, result);
            Debug.Assert(_currentNode != null, "current node is null");
        }

        private EBTEvaluateState? EvaluateDecorator(ref BTNode currentNode, bool startAtLast)
        {
            bool failureFlag = false;
            int length = _decoOwnerStack.Count;
            for (int i = startAtLast ? length - 1 : 0; i < length; i++)
            {
                var owner = _decoOwnerStack.Pop();

                bool failure = false;

                foreach (var decorator in owner.GetDecorators())
                {
                    EBTReEvauationState reEvauationState = decorator.EValuate();

                    if (reEvauationState == EBTReEvauationState.Failure)
                    {
                        var parent = owner.GetParent();
                        Debug.Assert(parent != null, "parent node can't be null");

                        failure = true;
                        currentNode = parent;
                        break;
                    }
                }

                if (failure == false)
                {
                    _decoOwnerStackSwap.Push(owner);
                }
                else // 데코레이터가 실패를 반환하면, attach된 노드의 cancel 처리
                {
                    failureFlag = true;
                    if (owner is IBTServiceOwner removingServiecOwner)
                    {
                        _serviceScheduler.TryRemoveOwner(removingServiecOwner);
                    }

                    if (owner is BTNode btNode)
                    {
                        btNode.Cancel();
                    }
                }
            }

            while (_decoOwnerStackSwap.Count > 0)
            {
                _decoOwnerStack.Push(_decoOwnerStackSwap.Pop());
            }
            

            if (failureFlag)
            {
                Debug.Assert(currentNode is not BTNAction);
                return EBTEvaluateState.Failure;
            }

            return null;
        }

        private BTNode EValuate(BTNode node, EBTEvaluateState? childEvaluateState = null)
        {
            Debug.Assert(node != null, "parameter is null");

            bool firstCome = true;
            while (true)
            {
                if (countD > 100000)
                {
                    Debug.Assert(false);
                    return null;
                }

                countD++;

                if (firstCome == false && node is BTNRoot)
                {
                    _tree.EventManager.Invoke(EBTBroadcastEvent.TreeArrivedRoot);
                    return node;
                }

                firstCome = false;

                if (childEvaluateState == null && node is IBTDecoratorOwner decoEntryOwner)
                {
                    if (_decoOwnerStack.Contains(decoEntryOwner) == false)
                    {
                        _decoOwnerStack.Push(decoEntryOwner);
                        childEvaluateState = EvaluateDecorator(ref node, true);
                    }
                }

                var result = node.EValuate(childEvaluateState);

                if (result.State != EBTEvaluateState.Running) // up evaluate 일 때
                {
                    if (node is IBTDecoratorOwner decoratorOwner)
                    {
                        foreach (var decorator in decoratorOwner.GetDecorators())
                        {
                            EBTReEvauationState s;

                            switch (result.State)
                            {
                                case EBTEvaluateState.Success:
                                    s = EBTReEvauationState.Success;
                                    break;
                                case EBTEvaluateState.Failure:
                                    s = EBTReEvauationState.Failure;
                                    break;
                                default:
                                    s = EBTReEvauationState.Failure;
                                    Debug.Assert(false, "decorator undefined state");
                                    break;
                            }

                            decorator.ReEvaluate(s);
                        }
                    }
                }


                if (result.State != EBTEvaluateState.Running)
                {
                    if (node is IBTServiceOwner removingServiecOwner)
                    {
                        _serviceScheduler.TryRemoveOwner(removingServiecOwner);
                    }
                }
                else
                {
                    if (node is IBTServiceOwner addingServiecOwner)
                    {
                        _serviceScheduler.TryAddOwner(addingServiecOwner);
                    }
                }

                var branchState = BranchNode(result, ref node, out childEvaluateState);

                if (branchState == EBranchNodeState.Up)
                {
                    continue;
                }
                else
                {
                    return node;
                }
            }

            Debug.Assert(false, "undefined state");
            return node;
        }

        
        private EBranchNodeState BranchNode(BTEvaluateResult result, ref BTNode node, out EBTEvaluateState? childEvaluateState)
        {
            if (result.State == EBTEvaluateState.Failure)
            {
                if (node is IBTDecoratorOwner)
                {
                    _decoOwnerStack.Pop();
                }

                var parent = node.GetParent();
                Debug.Assert(parent != null, "parent node can't be null");

                childEvaluateState = EBTEvaluateState.Failure;
                node = parent;

                return EBranchNodeState.Up;
            }

            
            if (result.State == EBTEvaluateState.Success)
            {
                if (node is IBTDecoratorOwner)
                {
                    _decoOwnerStack.Pop();
                }

                var parent = node.GetParent();
                Debug.Assert(parent != null, "parent node can't be null");

                childEvaluateState = EBTEvaluateState.Success;
                node = parent;
                return EBranchNodeState.Up;
            }

            childEvaluateState = null;
            if (result.State == EBTEvaluateState.Running)
            {
                if (node is BTNComposite or BTNRoot)
                {
                    node = result.ToEvaluateNode;
                    Debug.Assert(node != null, "composite's evaluation node can't be null at running state");
                    
                    return EBranchNodeState.Up;
                }

                if (node is BTNAction)
                {
                    return EBranchNodeState.Return;
                }

                Debug.Assert(false, $"undefined running state, cur node: {node.GetType().ToString()}");
                
                return EBranchNodeState.Return;
            }

            Debug.Assert(false, "undefined state");
            
            return EBranchNodeState.Return;
        }
    }


    public enum EBranchNodeState
    {
        Up,
        Return
    }

    public enum EBTReEvauationState
    {
        Failure,
        Success
    }

    public interface IBTDecoratorOwner
    {
        public List<BTNDecorator> GetDecorators();
        public BTNode GetParent();
    }

    public interface IBTServiceOwner
    {
        public List<BTNService> GetServices();
    }

    [System.Serializable]
    public enum EBTEvaluateState
    {
        Success,
        Running,
        Failure,
    }

    [System.Serializable]
    public struct BTEvaluateResult
    {
        public EBTEvaluateState State;

        [CanBeNull] public BTNode ToEvaluateNode;
    }

    public enum EBTBroadcastEvent
    {
        TreeArrivedRoot
    }

    public class BTBroadcastEvent
    {
    }

    public delegate void DBTBroadcastCallback(BTBroadcastEvent evt);

    public class BTEventManager
    {
        private Dictionary<EBTBroadcastEvent, List<DBTBroadcastCallback>> _callbackTable = new();

        private List<DBTBroadcastCallback> GetCallbackList(EBTBroadcastEvent evtType)
        {
            if (_callbackTable.TryGetValue(evtType, out var list))
            {
                return list;
            }
            else
            {
                list = new List<DBTBroadcastCallback>(10);
                _callbackTable.Add(evtType, list);
                return list;
            }
        }

        public void RegisterCallback(EBTBroadcastEvent evtType, DBTBroadcastCallback callback)
        {
            GetCallbackList(evtType).Add(callback);
        }

        public void UnRegisterCallback(EBTBroadcastEvent evtType, DBTBroadcastCallback callback)
        {
            GetCallbackList(evtType).Remove(callback);
        }

        public void Invoke(EBTBroadcastEvent evtType)
        {
            var evt = new BTBroadcastEvent();
            foreach (var callback in GetCallbackList(evtType))
            {
                callback(evt);
            }
        }
    }
}