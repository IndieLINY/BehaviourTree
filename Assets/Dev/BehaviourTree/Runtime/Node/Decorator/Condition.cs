using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IndieLINY.AI.BehaviourTree;
public class BTNDConditionBoolean : BTNDecorator
{
    public bool failure;
    
    public override EBTReEvauationState EValuate()
    {
        return failure ? EBTReEvauationState.Failure : EBTReEvauationState.Success;
    }
}
