using System.Collections;
using System.Collections.Generic;
using __Scripts.Systems;
using UnityEngine;
using UnityEngine.UI;
    
public enum EMenuPanel
{
    Warrior,
    Archer,
    Mage,
    Equip
}

public class MenuPanel : MonoBehaviour
{
    
    [SerializeField] private RectTransform WarriorPanel;
    [SerializeField] private RectTransform ArcherPanel;
    [SerializeField] private RectTransform MagePanel;   
    [SerializeField] private RectTransform EquipPanel;
    
    public void SwitchPanel(int panel)
    {
        SelectPanel((EMenuPanel)panel);
    }
    
    
    public void SelectPanel(EMenuPanel panel)
    {
        WarriorPanel.gameObject.SetActive(false);
        ArcherPanel.gameObject.SetActive(false);
        MagePanel.gameObject.SetActive(false);
        EquipPanel.gameObject.SetActive(false);

        AudioSystem.Instance.PlayMenuSelectSound();
        switch(panel)
        {
            case EMenuPanel.Warrior:
                WarriorPanel.gameObject.SetActive(true);
                break;
            case EMenuPanel.Archer:
                ArcherPanel.gameObject.SetActive(true);
                break;
            case EMenuPanel.Mage:
                MagePanel.gameObject.SetActive(true);
                break;
            case EMenuPanel.Equip:
                EquipPanel.gameObject.SetActive(true);
                break;
        }
    }
}
