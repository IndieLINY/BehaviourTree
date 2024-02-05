using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public class BTNRoot : ScriptableObject, IBTNRoot
    {
        [SerializeField] private IBTNode _child;
        
        public IBTNode GetParent() 
            => null;

        public IBTNode SetParent(IBTNode parent)
            => throw new System.NotImplementedException();

        public void AddChild(IBTNode node)
        {
            _child = node;
            node.SetParent(this);
        }

        public IBTNode GetChild()
            => _child;
        
        public BTEvaluateResult EValuate(EBTEvaluateState? childEvaluateState)
        {
            return new BTEvaluateResult()
            {
                State = EBTEvaluateState.Running,
                ToEvaluateNode = _child
            };
        }

        public void Init(BTMain treeMain)
        {
        }

    }
}