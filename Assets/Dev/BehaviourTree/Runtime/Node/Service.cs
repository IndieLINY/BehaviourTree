using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public interface IBTNService
    {
    }
    public interface IBTNSInterval : IBTNService
    {
        public void Update();
        public float GetInterval();
    }
    public abstract class BTNService : BTNode
    {
        [SerializeField] private float interval = 0.1f;

        public float Interval => interval;
        
        public sealed override BTEvaluateResult EValuate(EBTEvaluateState? upEvaluateState)
            => throw new System.NotImplementedException();
    }
}