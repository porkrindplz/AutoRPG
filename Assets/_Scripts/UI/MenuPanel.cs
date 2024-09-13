using System;
using System.Collections;
using System.Collections.Generic;
using __Scripts.Systems;
using _Scripts.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Logger = _Scripts.Utilities.Logger;

public enum EMenuPanel
{
    Sword,
    Shield,
    Staff,
    Slingshot
}

public class MenuPanel : MonoBehaviour
{
    
    [SerializeField] private RectTransform SwordPanel;
    [SerializeField] private RectTransform StaffPanel;
    [SerializeField] private RectTransform SlingshotPanel;   
    [SerializeField] private RectTransform ShieldPanel;

    [SerializeField] private Button ResetTreeButton;
    [SerializeField] private TextMeshProUGUI SkillPointText;
    
    private int openPanelId;

    private void Start()
    {
        //GameManager.Instance.OnUpgraded += (_, _) => UpdateSkillText();
        GameManager.Instance.OnBeforeGameStateChanged += (state, gameState) =>
        {
            if (gameState == EGameState.Playing)
            {
                //UpdateSkillText();
            }
        };
        GameManager.Instance.OnSkillPointGain += UpdateSkillText;
        GameManager.Instance.OnUpgraded += (_, _) => UpdateSkillText();
    }

    public void SwitchPanel(int panel)
    {
        if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;
        SelectPanel((EMenuPanel)panel);
        openPanelId = panel;
    }

    public void ResetTree()
    {
        if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;
        
        switch ((EMenuPanel)openPanelId)
        {
            case EMenuPanel.Sword:
                GameManager.Instance.AllTrees.Sword.ResetTree();
                break;
            case EMenuPanel.Staff:
                GameManager.Instance.AllTrees.Staff.ResetTree();
                break;
            case EMenuPanel.Slingshot:
                GameManager.Instance.AllTrees.Slingshot.ResetTree();
                break;
            case EMenuPanel.Shield:
                GameManager.Instance.AllTrees.Shield.ResetTree();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        UpdateSkillText();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchPanel(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchPanel(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchPanel(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchPanel(3);
    }
    
    void UpdateSkillText()
    {
        var player = (Player)GameManager.Instance.Player.Entity;
        SkillPointText.text = $"Skill Points: {player.UsedSkillPoints} / {player.MaxSkillPoints}";
    }

    public void SelectPanel(EMenuPanel panel)
    {
        SwordPanel.gameObject.SetActive(false);
        StaffPanel.gameObject.SetActive(false);
        SlingshotPanel.gameObject.SetActive(false);
        ShieldPanel.gameObject.SetActive(false);

        switch(panel)
        {
            case EMenuPanel.Sword:
                SwordPanel.gameObject.SetActive(true);
                break;
            case EMenuPanel.Staff:
                StaffPanel.gameObject.SetActive(true);
                break;
            case EMenuPanel.Slingshot:
                SlingshotPanel.gameObject.SetActive(true);
                break;
            case EMenuPanel.Shield:
                ShieldPanel.gameObject.SetActive(true);
                break;
        }
    }
    public void PlaySelectSound()
    {
        AudioSystem.Instance.PlayMenuSelectSound();
    }
}
