using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using Logger = _Scripts.Utilities.Logger;

public class VictoryPanel : MonoBehaviour
{
    [SerializeField] private Button nutsButton;
    [SerializeField] private Button healthButton;

    private int nuts;
    [SerializeField] int minNuts = 10;
    [SerializeField] int health = 250;
    private void OnEnable()
    {
        UpdateButtons();
        nutsButton.onClick.AddListener(CollectNuts);
        healthButton.onClick.AddListener(HealPlayer);
    }

    private void OnDisable()
    {
        nutsButton.onClick.RemoveAllListeners();
        healthButton.onClick.RemoveAllListeners();
    }

    private void UpdateButtons()
    {
        nutsButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Nuts: {GameManager.Instance.EnemyNuts}";
        nuts = GameManager.Instance.EnemyNuts;
        healthButton.GetComponentInChildren<TextMeshProUGUI>().text = $"+{health.ToString()} HP";
    }
    private void HealPlayer()
    {
        GameManager.Instance.Player.Entity.CurrentHealth += health;
        if(GameManager.Instance.Player.Entity.CurrentHealth > GameManager.Instance.Player.Entity.MaxHealth)
        {
            GameManager.Instance.Player.Entity.CurrentHealth = GameManager.Instance.Player.Entity.MaxHealth;
        }

        Continue();
    }

    private void CollectNuts()
    {
        GameManager.Instance.Player.Entity.Nuts += nuts;
        Continue();
    }

    public void Continue()
    {
        GameManager.Instance.ChangeGameState(EGameState.SpawnEnemy);
        this.gameObject.SetActive(false);
    }
}
