using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public abstract class BTNDecorator : BTNode
    {
        public sealed override BTEvaluateResult EValuate(EBTEvaluateState? upEvaluateState)
            => throw new System.NotImplementedException();

        public virtual EBTReEvauationState ReEvaluate(EBTReEvauationState reEvaluatingState)
        {
            return reEvaluatingState;
        }

        public virtual EBTReEvauationState EValuate()
        {
            return EBTReEvauationState.Success;
        }
    }
}