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
    
    public void SwitchPanel(int panel)
    {
        SelectPanel((EMenuPanel)panel);
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
