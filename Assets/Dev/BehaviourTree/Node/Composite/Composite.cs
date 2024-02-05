using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public abstract class BTNComposite : BTNode, IBTNComposite
    {
        [SerializeField] 
        protected List<IBTNode> childs = new List<IBTNode>();
        
        public void AddChild(IBTNode node)
        {
            childs.Add(node);
            node.SetParent(this);
        }

        public void RemoveChild(IBTNode node)
        {
            childs.Remove(node);
            node.SetParent(null);
        }

        public IEnumerable<IBTNode> GetChildAll()
        {
            return childs;
        }
    }
}