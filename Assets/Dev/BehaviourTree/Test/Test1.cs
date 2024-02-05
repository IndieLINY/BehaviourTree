using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieLINY.AI.BehaviourTree
{
    public class BTTest : MonoBehaviour
    {
        private BTExecutor _executor;

        void Awake()
        {
            var btMain = ScriptableObject.CreateInstance<BTMain>();
            btMain.Init();

            var test1_1 = ScriptableObject.CreateInstance<BTNATest1>();
            var test1_2 = ScriptableObject.CreateInstance<BTNATest1>();
            var test1_3 = ScriptableObject.CreateInstance<BTNATest1>();
            
            var test2_1 = ScriptableObject.CreateInstance<BTNATest1>();
            var test2_2 = ScriptableObject.CreateInstance<BTNATest1>();
            var test2_3 = ScriptableObject.CreateInstance<BTNATest1>();
            
            var sequence1 = ScriptableObject.CreateInstance<BTNCSequence>();
            var selector = ScriptableObject.CreateInstance<BTNCSelector>();
            var sequence2 = ScriptableObject.CreateInstance<BTNCSequence>();
            var root = ScriptableObject.CreateInstance<BTNRoot>();

            test1_1.number = 1;
            test1_2.number = 2;
            test1_3.number = 3;
            test1_3.failure = true;
            
            test2_1.number = 4;
            test2_2.number = 5;
            test2_2.failure = true;
            test2_3.number = 6;

            btMain.root = root;
            root.AddChild(sequence1);
            sequence1.AddChild(test1_1);
            sequence1.AddChild(test1_2);
            sequence1.AddChild(selector);
            sequence1.AddChild(test1_3);
            
            selector.AddChild(sequence2);
            
            sequence2.AddChild(test2_1);
            sequence2.AddChild(test2_2);
            selector.AddChild(test2_3);

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


            _executor = new BTExecutor(btMain);
        }

        void Update()
        {
            _executor.Update();
        }
    }

    public class BTNATest1 : BTNAction
    {
        public int number;
        public bool failure;
        private float timer = 0f;
        
        protected override EBTEvaluateState Update()
        {
            if (timer >= 1f)
            {
                timer = 0f;
                Debug.Log("test 2: " + number);
                return failure ? EBTEvaluateState.Failure : EBTEvaluateState.Success;
            }

            timer += Time.deltaTime;
            return EBTEvaluateState.Running;
        }
    }
}