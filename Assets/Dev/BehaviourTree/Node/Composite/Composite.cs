using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public abstract class BTNComposite : BTNode
    {
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
            if (childEvaluateState == null)
            {
                return DownEvaluate();
            }
            else
            {
                return UpEvaluate(childEvaluateState.Value);
            }
        }

        protected abstract BTEvaluateResult DownEvaluate();
        protected abstract BTEvaluateResult UpEvaluate(EBTEvaluateState childEvaluateState);


    }
}