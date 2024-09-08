using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VictoryPanel : MonoBehaviour
{
    [SerializeField] private Button nutsButton;
    [SerializeField] private Button healthButton;

    private int nuts;
    private int health = 10;
    private void OnEnable()
    {
        GameManager.Instance.OnBeforeGameStateChanged += GameStateChanged;
        nutsButton.onClick.AddListener(CollectNuts);
        healthButton.onClick.AddListener(HealPlayer);
    }
    
    void GameStateChanged(EGameState before, EGameState after)
    {
        if (after == EGameState.EnemyDefeated)
        {
            nuts = GameManager.Instance.EnemyManager.CurrentEnemy.Entity.Nuts;
            nutsButton.GetComponentInChildren<TextMeshProUGUI>().text = nuts.ToString();
            healthButton.GetComponentInChildren<TextMeshProUGUI>().text = health.ToString();
        }
    }

    public void HealPlayer()
    {
        GameManager.Instance.Player.Entity.CurrentHealth += health;
        if(GameManager.Instance.Player.Entity.CurrentHealth > GameManager.Instance.Player.Entity.MaxHealth)
        {
            GameManager.Instance.Player.Entity.CurrentHealth = GameManager.Instance.Player.Entity.MaxHealth;
        }

        Continue();
    }

    public void CollectNuts()
    {
        GameManager.Instance.Nuts += nuts;
        Continue();
    }

    public void Continue()
    {
        GameManager.Instance.ChangeGameState(EGameState.SpawnEnemy);
        this.gameObject.SetActive(false);
    }
}
