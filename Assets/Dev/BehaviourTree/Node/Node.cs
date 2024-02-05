using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public abstract class BTNode : ScriptableObject, IBTNode
    {
        [HideInInspector] public string guid;

        [HideInInspector] public Vector2 nodeViewPosition;
        [SerializeField] public List<BTNode> childs = new List<BTNode>();
        
        
        
        [SerializeField] 
        private IBTNode _parentNode;
        
        public BTMain TreeMain { get; private set; }
        
        

        public IBTNode GetParent() => _parentNode;
        public IBTNode SetParent(IBTNode parent) => _parentNode = parent;
        public abstract BTEvaluateResult EValuate(EBTEvaluateState? upEvaluateState);
        public virtual void Init(BTMain treeMain)
        {
            this.TreeMain = treeMain;
        }
    }
}