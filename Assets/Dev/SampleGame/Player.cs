using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class Player : MonoBehaviour
{
    [SerializeField]
    private AsyncReactiveProperty<int> _hp;

    public AsyncReactiveProperty<int> Hp => _hp;
}
