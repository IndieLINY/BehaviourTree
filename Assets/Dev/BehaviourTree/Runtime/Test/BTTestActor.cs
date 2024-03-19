using System.Collections;
using System.Collections.Generic;
using IndieLINY.AI.BehaviourTree;
using UnityEngine;

public class BTTestActor : MonoBehaviour
{
    public BTExecutor Executor;
    
    void Update()
    {
        Executor?.Update();
    }
}
