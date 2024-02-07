using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public abstract class BTNAction : BTNode, IBTDecoratorOwner, IBTServiceOwner
    {
        [SerializeField] public List<BTNService> services = new();
        [SerializeField] public List<BTNDecorator> decorators = new();

        public void Attach(BTNService service)
            => services.Add(service);
        public void Attach(BTNDecorator decorator)
            => decorators.Add(decorator);
        public sealed override BTNode GetParent()
            => base.GetParent();
        public sealed override void SetParent(BTNode node)
            => base.SetParent(node);

        public List<BTNDecorator> GetDecorators()
            => decorators;

        public List<BTNService> GetServices()
            => services;
    }

    public abstract class BTNActionSync : BTNAction
    {
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
    
    public abstract class BTNActionAsync : BTNAction
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
            await UniTask.Delay(1, cancellationToken: new CancellationToken());
            throw new NotImplementedException();
        }
    }
}
