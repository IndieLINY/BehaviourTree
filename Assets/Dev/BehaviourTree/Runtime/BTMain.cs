using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using IndieLINY.AI.BehaviourTree;

[CreateAssetMenu(menuName = "AI/BehaviourTree", fileName = "BehaviourTree")]
public class BTMain : ScriptableObject
{
    public BTNRoot root;

    public List<BTNode> nodes = new List<BTNode>();

    public BTEventManager EventManager { get; private set; }

    public void Init()
    {
        EventManager = new BTEventManager();
    }

    public BTNode CreateNode(System.Type type)
    {
        BTNode node = ScriptableObject.CreateInstance<BTNode>();

        node.name = "test";
        node.guid = GUID.Generate().ToString();
        nodes.Add(node);
        
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();

        return node;
    }

    public void DeleteNode(BTNode node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }
}
