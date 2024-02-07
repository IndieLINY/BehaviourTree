using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public abstract class BTNComposite : BTNode, IBTDecoratorOwner, IBTServiceOwner
    {
        [SerializeField] public List<BTNService> services = new();
        [SerializeField] public List<BTNDecorator> decorators = new();
        
        public void AddChild(BTNode node)
        {
            childs.Add(node);
            node.SetParent(this);
        }

        public void RemoveChild(BTNode node)
        {
            childs.Remove(node);
            node.SetParent(null);
        }

        public void Attach(BTNService service)
            => services.Add(service);
        public void Attach(BTNDecorator decorator)
            => decorators.Add(decorator);
        public sealed override BTNode GetParent()
            => base.GetParent();
        public sealed override void SetParent(BTNode node)
            => base.SetParent(node);
        
        public IEnumerable<BTNode> GetChildAll()
        {
            return childs;
        }

        public sealed override BTEvaluateResult EValuate(EBTEvaluateState? childEvaluateState)
        {
            BTEvaluateResult result = default;
            
            if (childEvaluateState == null)
            { 
                result = DownEvaluate();
            }
            else
            {
                result = UpEvaluate(childEvaluateState.Value);
            }

            return result;
        }

        public List<BTNDecorator> GetDecorators()
            => decorators;

        public List<BTNService> GetServices()
            => services;

        protected abstract BTEvaluateResult DownEvaluate();
        protected abstract BTEvaluateResult UpEvaluate(EBTEvaluateState childEvaluateState);


    }
}