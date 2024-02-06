using System.Collections;
using System.Collections.Generic;
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

        public BTExecutor(BTMain treeMain)
        {
            this._rootNode = treeMain.root;
            this._currentNode = treeMain.root;
            this._tree = treeMain;
            
            treeMain.Init();
            
            foreach(var node in treeMain.nodes)
            {
                node.Init(treeMain);
            }
        }

        public void Update()
        {
            _currentNode = EValuate(_currentNode);
            Debug.Assert(_currentNode != null, "current node is null");
        }

        private BTNode EValuate(BTNode node)
        {
            Debug.Assert(node != null, "parameter is null");

            bool debugDone = false;
            EBTEvaluateState? childEvaluateState = null;

            while (debugDone == false)
            {
                var result = node.EValuate(childEvaluateState);


                if (result.State == EBTEvaluateState.Failure)
                {
                    var parent = node.GetParent();
                    Debug.Assert(parent != null, "parent node can't be null");
                    if (parent is BTNRoot)
                    {
                        _tree.EventManager.Invoke(EBTBroadcastEvent.TreeArrivedRoot);
                        return parent;
                    }

                    childEvaluateState = EBTEvaluateState.Failure;
                    node = parent;

                    continue;
                }
                
                if (result.State == EBTEvaluateState.Success)
                {
                    var parent = node.GetParent();
                    Debug.Assert(parent != null, "parent node can't be null");
                    if (parent is BTNRoot)
                    {
                        _tree.EventManager.Invoke(EBTBroadcastEvent.TreeArrivedRoot);
                        return parent;
                    }


                    childEvaluateState = EBTEvaluateState.Success;
                    node = parent;
                    continue;
                }
                
                if (result.State == EBTEvaluateState.Running)
                {
                    if (node is BTNComposite || node is BTNRoot)
                    {
                        childEvaluateState = null;
                        node = result.ToEvaluateNode;
                        Debug.Assert(node != null, "composite's evaluation node can't be null at running state");
                        continue;
                    }
                    if (node is BTNAction || node is BTNActionAsync)
                    {
                        return node;
                    }
                    
                    Debug.Assert(false, $"undefined running state, cur node: {node.GetType().ToString()}");
                    return node;
                }
                
                Debug.Assert(false, "undefined state");
                return node;
            }

            Debug.Assert(false, "undefined state");
            return node;
        }
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

    public abstract class BTBlackboardBase
    {
        public T GetData<T>(string key)
        {
            return default;
        }
    }
}