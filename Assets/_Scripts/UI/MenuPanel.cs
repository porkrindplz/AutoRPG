using System;
using System.Collections;
using System.Collections.Generic;
using __Scripts.Systems;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
    
public enum EMenuPanel
{
    Sword,
    Staff,
    Slingshot,
    Shield
}

public class MenuPanel : MonoBehaviour
{
    
    [SerializeField] private RectTransform SwordPanel;
    [SerializeField] private RectTransform StaffPanel;
    [SerializeField] private RectTransform SlingshotPanel;   
    [SerializeField] private RectTransform ShieldPanel;

    [SerializeField] private Button ResetTreeButton;

    private int openPanelId;
    
    public void SwitchPanel(int panel)
    {
        SelectPanel((EMenuPanel)panel);
        openPanelId = panel;
    }

    public void ResetTree()
    {
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
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchPanel(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchPanel(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchPanel(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchPanel(3);
    }

    public void SelectPanel(EMenuPanel panel)
    {
        SwordPanel.gameObject.SetActive(false);
        StaffPanel.gameObject.SetActive(false);
        SlingshotPanel.gameObject.SetActive(false);
        ShieldPanel.gameObject.SetActive(false);

        AudioSystem.Instance.PlayMenuSelectSound();
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
}
