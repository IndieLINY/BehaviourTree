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
        private IBTNRoot _rootNode;
        private IBTNode _currentNode;
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

        private IBTNode EValuate(IBTNode node)
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
                    if (parent is IBTNRoot)
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
                    if (parent is IBTNRoot)
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
                    if (node is IBTNComposite || node is IBTNRoot)
                    {
                        childEvaluateState = null;
                        node = result.ToEvaluateNode;
                        Debug.Assert(node != null, "composite's evaluation node can't be null at running state");
                        continue;
                    }
                    if (node is IBTNAction)
                    {
                        return node;
                    }
                    
                    
                    Debug.Assert(false, "undefined running state");
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

        [CanBeNull] public IBTNode ToEvaluateNode;
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
    

    public interface IBTNode
    {
        public IBTNode GetParent();
        public IBTNode SetParent(IBTNode parent);
        public BTEvaluateResult EValuate(EBTEvaluateState? childEvaluateState);

        public void Init(BTMain treeMain);
    }

    public interface IBTNRoot : IBTNode
    {
        public void SetChild(IBTNode node);
        public IBTNode GetChild();
    }

    public interface IBTNAttach : IBTNode
    {
        public void Attach(IBTNComposite compositeNode);
        public void Attach(IBTNAction actionNode);
    }

    public interface IBTNDecorator : IBTNAttach
    {
    }

    public interface IBTNService : IBTNAttach
    {
    }

    public interface IBTNComposite : IBTNode
    {
        public void AddChild(IBTNode node);
        public void RemoveChild(IBTNode node);
        public IEnumerable<IBTNode> GetChildAll();
    }

    public interface IBTNAction : IBTNode
    {
    }

    public abstract class BTBlackboardBase
    {
        public T GetData<T>(string key)
        {
            return default;
        }
    }
}