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
    [SerializeField] int healthWithoutNuts = 300;
    [SerializeField] private int healthWithNuts = 100;
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
        nutsButton.GetComponentInChildren<TextMeshProUGUI>().text = $"HP: +{healthWithNuts}\n+{GameManager.Instance.EnemyNuts} Nuts";
        nuts = GameManager.Instance.EnemyNuts;
        healthButton.GetComponentInChildren<TextMeshProUGUI>().text = $"HP: +{healthWithoutNuts.ToString()}\n+{(int)(nuts*0.5)} Nuts";
    }
    private void HealPlayer()
    {
        GameManager.Instance.Player.Entity.CurrentHealth += healthWithoutNuts;
        GameManager.Instance.Player.Entity.Nuts += (int)(0.5 * nuts);
        Continue();
    }

    private void CollectNuts()
    {
        GameManager.Instance.Player.Entity.CurrentHealth += healthWithNuts;
        GameManager.Instance.Player.Entity.Nuts += nuts;
        Continue();
    }

    public void Continue()
    {
        var e = GameManager.Instance.Player.Entity;
        e.CurrentHealth = Math.Min(e.CurrentHealth, e.MaxHealth);
        GameManager.Instance.ChangeGameState(EGameState.SpawnEnemy);
        this.gameObject.SetActive(false);
    }
}
