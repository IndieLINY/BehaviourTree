using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public class BTNRoot : BTNode
    {
        [SerializeField] private BTNode _child;
        
        public override BTNode GetParent() 
            => null;

        public override void SetParent(BTNode parent)
            => throw new System.NotImplementedException();

        public void SetChild(BTNode node)
        {
            _child = node;
            node.SetParent(this);
        }

        public BTNode GetChild()
            => _child;
        
        public override BTEvaluateResult EValuate(EBTEvaluateState? childEvaluateState)
        {
            return new BTEvaluateResult()
            {
                State = EBTEvaluateState.Running,
                ToEvaluateNode = _child
            };
        }

        public override void Init(BTMain treeMain)
        {
        }

    }
}