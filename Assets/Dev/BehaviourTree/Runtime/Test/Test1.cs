using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    using ACTION_TEST = BTNATest1Async;
    public class BTTest : MonoBehaviour
    {
        private BTExecutor _executor;

        void Awake()
        {
            var btMain = ScriptableObject.CreateInstance<BTMain>();
            btMain.Init();

            var test1_1 = ScriptableObject.CreateInstance<ACTION_TEST>();
            var test1_2 = ScriptableObject.CreateInstance<ACTION_TEST>();
            var test1_3 = ScriptableObject.CreateInstance<ACTION_TEST>();
            
            var test2_1 = ScriptableObject.CreateInstance<ACTION_TEST>();
            var test2_2 = ScriptableObject.CreateInstance<BTNATest2Async>();
            var test2_3 = ScriptableObject.CreateInstance<ACTION_TEST>();
            var test2_4 = ScriptableObject.CreateInstance<ACTION_TEST>();

            var decoratorCondition = ScriptableObject.CreateInstance<BTNDConditionTestObserveTest2>();
            var serviceSec = ScriptableObject.CreateInstance<BTNSTestSecLog>();
            
            var sequence1 = ScriptableObject.CreateInstance<BTNCSequence>();
            var selector = ScriptableObject.CreateInstance<BTNCSelector>();
            var sequence2 = ScriptableObject.CreateInstance<BTNCSequence>();
            var root = ScriptableObject.CreateInstance<BTNRoot>();

            test1_1.number = 1;
            test1_2.number = 2;
            test1_3.number = 6;
            test1_3.failure = true;
            
            test2_1.number = 3;
            test2_2.number = 4;
            test2_2.failure = false;
            test2_3.number = 5;
            test2_4.number = 999;

            //decoratorCondition.failure = true;
            
            btMain.root = root;
            root.SetChild(sequence1);
            sequence1.AddChild(test1_1);
            sequence1.AddChild(test1_2);
            sequence1.AddChild(selector);
            sequence1.AddChild(test1_3);
            sequence1.Attach(serviceSec);
            
            selector.AddChild(sequence2);
            selector.AddChild(test2_3);
            selector.AddChild(test2_4);
            
            sequence2.AddChild(test2_1);
            sequence2.AddChild(test2_2);
            BTNATest2Async.fail = false;
            sequence2.Attach(decoratorCondition);

            btMain.root = root;
            btMain.nodes.Add(sequence1);
            btMain.nodes.Add(selector);
            btMain.nodes.Add(sequence2);
            btMain.nodes.Add(test1_1);
            btMain.nodes.Add(test1_2);
            btMain.nodes.Add(test1_3);
            btMain.nodes.Add(test2_1);
            btMain.nodes.Add(test2_2);
            btMain.nodes.Add(test2_3);
            btMain.nodes.Add(test2_4);
            btMain.nodes.Add(decoratorCondition);
            btMain.nodes.Add(serviceSec);


            _executor = new BTExecutor(btMain);
        }

        void Update()
        {
            _executor.Update();
        }
    }

    public class BTNATest1 : BTNActionSync
    {
        public int number;
        public bool failure;
        private float timer = 0f;
        
        protected override EBTEvaluateState Update()
        {
            if (timer >= 1f)
            {
                timer = 0f;
                Debug.Log("test 1: " + number);
                return failure ? EBTEvaluateState.Failure : EBTEvaluateState.Success;
            }

            timer += Time.deltaTime;
            return EBTEvaluateState.Running;
        }
    }

    public class BTNATest1Async : BTNActionAsync
    {
        public int number;
        public bool failure;

        protected override async UniTask<EBTEvaluateState> UpdateAsync(CancellationTokenSource token)
        {
            await UniTask.Delay(1000);
            
            Debug.Log("test 1: " + number);
            return failure ? EBTEvaluateState.Failure : EBTEvaluateState.Success;
        }
    }
    public class BTNATest2Async : BTNActionAsync
    {
        public int number;
        public bool failure;

        static public bool fail = false;

        protected override async UniTask<EBTEvaluateState> UpdateAsync(CancellationTokenSource token)
        {
            await UniTask.Delay(2000, cancellationToken:token.Token);
            
            if(token.IsCancellationRequested)
                return failure ? EBTEvaluateState.Failure : EBTEvaluateState.Success;
            
            fail = true;
            Debug.Log("set fail");
            
            await UniTask.Delay(10000, cancellationToken:token.Token);
            
            if(token.IsCancellationRequested)
                return failure ? EBTEvaluateState.Failure : EBTEvaluateState.Success;
            
            Debug.Log("test 2: " + number);
            
            return failure ? EBTEvaluateState.Failure : EBTEvaluateState.Success;
            
        }

        public override void OnCanceled()
        {
            Debug.Log("canceled");
        }
    }
    public class BTNDConditionTestObserveTest2 : BTNDecorator
    {
    
        public override EBTReEvauationState EValuate()
        {
            if (BTNATest2Async.fail)
                Debug.Log("FA!!");
            
            return BTNATest2Async.fail ? EBTReEvauationState.Failure : EBTReEvauationState.Success;
        }
    }

    public class BTNSTestSecLog : BTNService, IBTNSInterval
    {
        public void Update()
        {
//            Debug.Log("service");
        }

        public float GetInterval() => 1f;
    }
}