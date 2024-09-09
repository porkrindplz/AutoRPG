using System;
using _Scripts.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class SkillNode : MonoBehaviour
    {
        private string parentName;
        [SerializeField] private string UpgradeId;
        [SerializeField] private TextMeshProUGUI numText;
        private Button button;
        
        private void Awake()
        {
            parentName = transform.parent.name;
            button = GetComponent<Button>();
            GameManager.Instance.OnUpgraded += (_, _) =>
            {
                SetInteractable();
            };
            GameManager.Instance.OnResetTree += tree =>
            {
                if (parentName != null && parentName.Contains(tree.Name))
                {
                    numText.text = "0";
                    SetInteractable();
                }
            };
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
    }
}
