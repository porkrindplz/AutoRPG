using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryPanel : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.OnBeforeGameStateChanged += AfterGameStateChanged;
    }
    
    void AfterGameStateChanged(EGameState before, EGameState after)
    {

    }

    public void HealPlayer()
    {
        
    }

    public void CollectNuts()
    {
        
    }
}
