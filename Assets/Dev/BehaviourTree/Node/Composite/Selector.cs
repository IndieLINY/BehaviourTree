using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public class BTNCSelector : BTNComposite
    {
        private int _currentIndex;
        public override void Init(BTMain treeMain)
        {
            base.Init(treeMain);

            _currentIndex = 0;
            
            treeMain.EventManager.RegisterCallback(EBTBroadcastEvent.TreeArrivedRoot, OnArrivedRoot);
        }

        private void OnArrivedRoot(BTBroadcastEvent evt)
        {
            _currentIndex = 0;
        }

        protected override BTEvaluateResult DownEvaluate()
        {
            
            if (childs.Count <= _currentIndex)
            {
                return new BTEvaluateResult()
                {
                    State = EBTEvaluateState.Failure,
                    ToEvaluateNode = null
                };
            }
            
            return new BTEvaluateResult()
            {
                State = EBTEvaluateState.Running,
                ToEvaluateNode = childs[_currentIndex]
            };
        }

        protected override BTEvaluateResult UpEvaluate(EBTEvaluateState childEvaluateState)
        {
            if (childEvaluateState == EBTEvaluateState.Running)
            {
                return new BTEvaluateResult()
                {
                    State = EBTEvaluateState.Running,
                    ToEvaluateNode = childs[_currentIndex]
                };
            }
                
            if (childEvaluateState == EBTEvaluateState.Success)
            {
                return new BTEvaluateResult()
                {
                    State = EBTEvaluateState.Success,
                    ToEvaluateNode = null
                };
            }
                
            if (childEvaluateState == EBTEvaluateState.Failure)
            {
                if (childs.Count <= _currentIndex + 1)
                {
                    return new BTEvaluateResult()
                    {
                        State = EBTEvaluateState.Failure,
                        ToEvaluateNode = null
                    };  
                }
                    
                return new BTEvaluateResult()
                {
                    State = EBTEvaluateState.Running,
                    ToEvaluateNode = childs[++_currentIndex]
                };
            }

            Debug.Assert(false, "selector's undefined state");
            return default;
        }
    }
}