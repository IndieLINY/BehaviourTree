using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public abstract class BTNode : ScriptableObject
    {
        [HideInInspector] public string guid;

        [HideInInspector] public Vector2 nodeViewPosition;
        [SerializeField] public List<BTNode> childs = new();

        [SerializeField] private BTNode _parentNode;
        
        public BTMain TreeMain { get; private set; }
        public virtual BTNode GetParent() 
            => _parentNode;
        public virtual void SetParent(BTNode parent) 
            => _parentNode = parent;
        
        
        public abstract BTEvaluateResult EValuate(EBTEvaluateState? upEvaluateState);

        public virtual void Init(BTMain treeMain)
        {
            this.TreeMain = treeMain;
        }

        public void Cancel()
        {
            OnCanceled();
            foreach (var child in childs)
            {
                child.Cancel();
            }
        }

        public virtual void OnCanceled()
        {
        }
    }
}