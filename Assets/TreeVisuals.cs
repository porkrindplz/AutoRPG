using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Models;
using UnityEngine;
using UnityEngine.UI;
public class TreeVisuals : MonoBehaviour
{
    [SerializeField] private RectTransform[] upgradeButtons;
    [SerializeField] private Image cooldownImage;

    private float initWidth;
    private void Awake()
    {
        cooldownImage = transform.Find("CoolDown").GetComponent<Image>();
        initWidth = cooldownImage.rectTransform.rect.width;
        cooldownImage.enabled = false;
        GameManager.Instance.OnResetTree += ResetTree;

    }
    

    private void OnEnable()
    {
        
    }
    
    private void OnDisable()
    {
    }
    
    private void ResetTree(UpgradeTree tree)
    {
 
        Button[] buttons = new Button[upgradeButtons.Length];
        if(tree.Name != name) return;
        for (int i = 0;i<upgradeButtons.Length;i++)
        {
            buttons[i] = upgradeButtons[i].GetComponent<Button>();
            buttons[i].enabled = false;
        }
        cooldownImage.enabled = true;
        StartCoroutine(Cooldown(buttons, tree));
    }

    IEnumerator Cooldown(Button[] buttons, UpgradeTree tree)
    {
       // float duration = tree.respecCooldown;
       float duration = 1;
        float time = 0;
        cooldownImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initWidth);
        while(time<duration)
        {
            time += Time.deltaTime;
            cooldownImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initWidth * (time/duration));
            yield return null;
        }
        for (int i = 0;i<buttons.Length;i++)
        {
            buttons[i].enabled = true;
        }
        cooldownImage.enabled = false;
    }
}
