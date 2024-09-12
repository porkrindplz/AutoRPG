using System;
using _Scripts.Models;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class SkillNode : MonoBehaviour
    {
        [SerializeField] private string parentName;
        [SerializeField] private string UpgradeId;
        [SerializeField] private TextMeshProUGUI numText;
        private Button button;
        
        private void Awake()
        {
            // EW nasty hack TODO fix me
            bool currentActivity = transform.parent.gameObject.activeInHierarchy;
            transform.parent.gameObject.SetActive(true);
            parentName = transform.parent.name;
            transform.parent.gameObject.SetActive(currentActivity);

            button = GetComponent<Button>();
            button.interactable = false;
            GameManager.Instance.OnBeforeGameStateChanged += (state, gameState) =>
            {
                if (gameState == EGameState.Playing)
                {
                    SetInteractable();
                }
            };
            GameManager.Instance.OnUpgraded += (_, _) =>
            {
                SetInteractable();
            };
            GameManager.Instance.OnResetTree += tree =>
            {
                SetInteractable();
                if (parentName != null && parentName.Contains(tree.Name))
                {
                    numText.text = "0";
                    
                }
            };
        }

        private void OnEnable()
        {
            SetInteractable();
        }

        private void SetInteractable()
        {
            button.interactable = parentName switch
            {
                "SwordTree" => GameManager.Instance.AllTrees.Sword.CanUpgrade(UpgradeId),
                "StaffTree" => GameManager.Instance.AllTrees.Staff.CanUpgrade(UpgradeId),
                "SlingshotTree" => GameManager.Instance.AllTrees.Slingshot.CanUpgrade(UpgradeId),
                "ShieldTree" => GameManager.Instance.AllTrees.Shield.CanUpgrade(UpgradeId),
                _ => button.interactable
            };
        }

        public void OnClick()
        {
            if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;
            Upgrade upgrade = parentName switch
            {
                "SwordTree" => GameManager.Instance.AllTrees.Sword.TryUpgrade(UpgradeId),
                "StaffTree" => GameManager.Instance.AllTrees.Staff.TryUpgrade(UpgradeId),
                "SlingshotTree" => GameManager.Instance.AllTrees.Slingshot.TryUpgrade(UpgradeId),
                "ShieldTree" => GameManager.Instance.AllTrees.Shield.TryUpgrade(UpgradeId),
                _ => null
            };

            if (upgrade != null)
            {
                numText.text = upgrade.NumOfUpgrades.ToString();
            }
        }

        public void ShowToolTip()
        {
            string description = parentName switch
            {
                "SwordTree" => GameManager.Instance.AllTrees.Sword.Upgrades.Find(u => u.Id == UpgradeId).Description,
                "StaffTree" => GameManager.Instance.AllTrees.Staff.Upgrades.Find(u => u.Id == UpgradeId).Description,
                "SlingshotTree" => GameManager.Instance.AllTrees.Slingshot.Upgrades.Find(u => u.Id == UpgradeId).Description,
                "ShieldTree" => GameManager.Instance.AllTrees.Shield.Upgrades.Find(u => u.Id == UpgradeId).Description,
                _ => ""
            };
            ToolTip.Instance.ShowToolTip(description);
        }
        public void HideToolTip()
        {
            ToolTip.Instance.HideToolTip();
        }
    }
}
