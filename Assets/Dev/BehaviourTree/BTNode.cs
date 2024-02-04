using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNode : ScriptableObject
{
    [HideInInspector] public string guid;

    [HideInInspector] public Vector2 nodeViewPosition;
    [SerializeField] public List<BTNode> childs = new List<BTNode>();
}