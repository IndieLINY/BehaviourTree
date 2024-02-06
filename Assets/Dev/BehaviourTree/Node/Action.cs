using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{

    public abstract class BTNAction : BTNode
    {
        public sealed override BTNode GetParent()
            => base.GetParent();

        public sealed override void SetParent(BTNode node)
            => base.SetParent(node);
        
        public sealed override BTEvaluateResult EValuate(EBTEvaluateState? upEvaluateState)
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
    
    public abstract class BTNActionAsync : BTNode
    {
        private UniTask<EBTEvaluateState>? _task;
        public sealed override BTEvaluateResult EValuate(EBTEvaluateState? upEvaluateState)
        {
            EBTEvaluateState state = EBTEvaluateState.Running;

            if (_task == null)
            {
                _task = UpdateAsync();
            }
            else
            {
                if (_task.Value.Status.IsCompleted())
                {
                    state = _task.Value.AsValueTask().Result;
                    _task = null;
                }
                else if (_task.Value.Status.IsCanceled())
                {
                    state = EBTEvaluateState.Failure;
                    _task = null;
                }
            }

            return new()
            {
                State = state,
                ToEvaluateNode = null
            };
        }

        protected virtual async UniTask<EBTEvaluateState> UpdateAsync()
        {
            await UniTask.Delay(1);
            throw new NotImplementedException();
        }
    }
}
