using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public Player player;
    [SerializeField] private Text _text;

    private void Awake()
    {
        
    }

    private async UniTaskVoid OnObserve()
    {
        while (true)
        {
            int hp = await player.Hp.WaitAsync();

            _text.text = hp.ToString();
        }
    }
}
