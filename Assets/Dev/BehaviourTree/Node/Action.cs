using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public abstract class BTNAction : BTNode, IBTNAction
    {
        public override BTEvaluateResult EValuate(EBTEvaluateState? upEvaluateState)
        {
            EBTEvaluateState state = Update();

            return new()
            {
                State = state,
                ToEvaluateNode = null
            };
        }

        protected abstract EBTEvaluateState Update();
    }
}
