using System;
using System.Collections;
using System.Collections.Generic;
using IndieLINY.AI.BehaviourTree;
using UnityEngine;
using NUnit;
using NUnit.Framework;
using UnityEngine.TestTools;

public class TestCase
{
    private BTTestActor CreateTestActor()
    {
        var obj = new GameObject("BT Test actor");
        var com = obj.AddComponent<BTTestActor>();
        if (!com)
        {
            throw new Exception("failed to create Test actor");
        }

        return com;
    }

    private BTMain CreateBT()
    {
        var btMain = ScriptableObject.CreateInstance<BTMain>();
        btMain.Init();
        
        return btMain;
    }

    private void Init(out BTMain btMain)
    {
        btMain = CreateBT();
    }
    private void Start(BTMain btMain, out BTTestActor actor)
    {
        var executor = new BTExecutor(btMain);
        
        actor = CreateTestActor();
        actor.Executor = executor;
    }
    private void End(BTTestActor actor)
    {
        GameObject.Destroy(actor);
    }

    private bool CheckQueue(Queue<int> queue, List<int> matchList)
    {
        int curIndex = 0;
        while (queue.Count > 0)
        {
            int value = queue.Dequeue();

            if (matchList.Count <= curIndex || value != matchList[curIndex])
            {
                return false;
            }

            curIndex++;
        }

        return true;
    }
    
    [UnityTest]
    public IEnumerator Test1()
    {
        Init(out var btMain);
        
        var root = ScriptableObject.CreateInstance<BTNRoot>();
        var sequence = ScriptableObject.CreateInstance<BTNCSequence>();
        var test1 = ScriptableObject.CreateInstance<BTNATest1>();
        var test2 = ScriptableObject.CreateInstance<BTNATest1>();
        var test3 = ScriptableObject.CreateInstance<BTNATest1>();

        BTNATest1. COUNTER = new Queue<int>();
        
        root.SetChild(sequence);
        
        sequence.AddChild(test1);
        sequence.AddChild(test2);
        sequence.AddChild(test3);
        
        btMain.root = root;
        btMain.nodes.Add(sequence);
        btMain.nodes.Add(test1);
        btMain.nodes.Add(test2);
        btMain.nodes.Add(test3);


        test1.number = 1;
        test2.number = 2;
        test3.number = 3;
        
        Start(btMain, out var actor);

        yield return null;

        if (!CheckQueue(BTNATest1.COUNTER, new List<int>
            {
                1, 2, 3
            }))
        {
            throw new Exception("fail");
        }
        
        
        End(actor);

    }

    [UnityTest]
    public IEnumerator Test2()
    {
        Init(out var btMain);
        
        var root = ScriptableObject.CreateInstance<BTNRoot>();
        var sequence = ScriptableObject.CreateInstance<BTNCSequence>();
        var selector = ScriptableObject.CreateInstance<BTNCSelector>();
        var test1 = ScriptableObject.CreateInstance<BTNATest1>();
        var test2 = ScriptableObject.CreateInstance<BTNATest1>();
        var test3 = ScriptableObject.CreateInstance<BTNATest1>();
        var test4 = ScriptableObject.CreateInstance<BTNATest1>();

        BTNATest1. COUNTER = new Queue<int>();
        
        root.SetChild(sequence);
        
        sequence.AddChild(test1);
        sequence.AddChild(test2);
        sequence.AddChild(selector);
        
        selector.AddChild(test3);
        selector.AddChild(test4);
        
        btMain.root = root;
        btMain.nodes.Add(sequence);
        btMain.nodes.Add(selector);
        btMain.nodes.Add(test1);
        btMain.nodes.Add(test2);
        btMain.nodes.Add(test3);
        btMain.nodes.Add(test4);


        test1.number = 1;
        test2.number = 2;
        test3.number = 3;
        test4.number = 4;
        
        Start(btMain, out var actor);

        yield return new WaitUntil(() => BTNATest1.COUNTER.Count >= 3);

        if (!CheckQueue(BTNATest1.COUNTER, new List<int>
            {
                1, 2, 3
            }))
        {
            throw new Exception("fail");
        }
        
        End(actor);
    }

    [UnityTest]
    public IEnumerator Test3()
    {
        Init(out var btMain);
        
        var root = ScriptableObject.CreateInstance<BTNRoot>();
        var sequence = ScriptableObject.CreateInstance<BTNCSequence>();
        var selector = ScriptableObject.CreateInstance<BTNCSelector>();
        var test1 = ScriptableObject.CreateInstance<BTNATest1>();
        var test2 = ScriptableObject.CreateInstance<BTNATest1>();
        var test3 = ScriptableObject.CreateInstance<BTNATest1>();
        var test4 = ScriptableObject.CreateInstance<BTNATest1>();

        BTNATest1. COUNTER = new Queue<int>();
        
        root.SetChild(sequence);
        
        sequence.AddChild(test1);
        sequence.AddChild(test2);
        sequence.AddChild(selector);
        
        selector.AddChild(test3);
        selector.AddChild(test4);
        
        btMain.root = root;
        btMain.nodes.Add(sequence);
        btMain.nodes.Add(selector);
        btMain.nodes.Add(test1);
        btMain.nodes.Add(test2);
        btMain.nodes.Add(test3);
        btMain.nodes.Add(test4);


        test1.number = 1;
        test2.number = 2;
        test3.number = 3;
        test4.number = 4;
        
        Start(btMain, out var actor);

        yield return new WaitUntil(() => BTNATest1.COUNTER.Count >= 3);

        if (!CheckQueue(BTNATest1.COUNTER, new List<int>
            {
                1, 2, 3
            }))
        {
            throw new Exception("fail");
        }
        
        End(actor);
    }
}


public class BTNATest1 : BTNActionSync
{
    public int number;
    public bool failure;

    public static Queue<int> COUNTER;
        
    protected override EBTEvaluateState Update()
    {
        COUNTER.Enqueue(number);
        Debug.Log(number);
        return failure ? EBTEvaluateState.Failure : EBTEvaluateState.Success;
    }
}