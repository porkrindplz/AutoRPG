using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using __Scripts.Systems;
using JetBrains.Annotations;
using UnityEngine;

namespace _Scripts.Models
{

    [Serializable]
    public class AllTrees
    {
        public UpgradeTree Sword;
        public UpgradeTree Shield;
        public UpgradeTree Staff;
        public UpgradeTree Slingshot;

        public void Reset()
        {
            Sword.ResetTree();
            Shield.ResetTree();
            Staff.ResetTree();
            Slingshot.ResetTree();
        }
    }
    
    [Serializable]
    public class UpgradeTree
    {
        public List<Upgrade> Upgrades;
        public string Name;
        private bool active = true;

        private float respecCooldown = 1.5f;
        /// <summary>
        /// Checks if the given hero's upgrades allows for the new upgrade to be chosen.
        /// Criteria is based on if the skill tree contains the given id, all its prereqs have been researched,
        /// and there are still more possible upgrades that could be chosen
        /// </summary>
        public bool CanUpgrade(string id)
        {
            var player = GameManager.Instance.Player.Entity as Player;
            if (player?.UsedSkillPoints >= player?.MaxSkillPoints) return false;
            
            var currentUpgrades = Upgrades.Where(u => u.NumOfUpgrades > 0).ToList();
            var newUpgrade = Upgrades.FirstOrDefault(u => u.Id == id);
            if (newUpgrade is null) return false;

            if (!newUpgrade.CanUpgradeMore()) return false;
            
            foreach(var exclusive in newUpgrade.ExclusiveWith)
            {
                if (currentUpgrades.FindIndex(u=>u.Id ==exclusive)!=-1)
                {
                    return false;
                }
            }

            // If condition is and, then all are required. Also works for all non-defined prerequsuite types with 1
            if (newUpgrade.PrerequisiteType is "and" or null or "")
            {
                foreach (var prereq in newUpgrade.Prerequisites)
                {
                    if (currentUpgrades.FindIndex(u => u.Id == prereq) == -1)
                    {
                        return false;
                    }
                }
            }

            // If condition is or, then only one is required
            if (newUpgrade.PrerequisiteType == "or")
            {
                foreach (var prereq in newUpgrade.Prerequisites)
                {
                    if (currentUpgrades.FindIndex(u => u.Id == prereq) != -1)
                    {
                        return true;
                    }
                }

                return false;
            }
            return true;
        }

        [CanBeNull]
        public Upgrade GetHighestLevelAction()
        {
            var boughtUpgrade = Upgrades?.Where(u => u.NumOfUpgrades > 0);
            if (boughtUpgrade == null || !boughtUpgrade.Any()) return null;

            int highestLevel = 0;
            Upgrade? highestUpgrade = null;
            foreach (var upgrade in boughtUpgrade)
            {
                if (upgrade.Level != null && upgrade.Level > highestLevel)
                {
                    highestLevel = upgrade.Level;
                    highestUpgrade = upgrade;
                }
            }

            return highestUpgrade;
        }

        public void ResetTree()
        {
            var pointsToGiveBack = Upgrades?.Sum(u => u.NumOfUpgrades) ?? 0;
            ((Player)GameManager.Instance.Player.Entity).UsedSkillPoints -= pointsToGiveBack;
            Upgrades?.ForEach(u => u.NumOfUpgrades = 0);
            GameManager.Instance.OnResetTree?.Invoke(this);

            GameManager.Instance.StartCoroutine(RespecCoroutine());
        }
        
        IEnumerator RespecCoroutine()
        {
            active = false;
            yield return new WaitForSecondsRealtime(respecCooldown);
            active = true;
        }

        public int GetUpgradeLevel(string id)
        {
            var upgrade = Upgrades.FirstOrDefault(u => u.Id == id);
            return upgrade?.NumOfUpgrades ?? 0;
        }
        
        public Upgrade? TryUpgrade(string id)
        {
            if (!CanUpgrade(id)|| !active)
            {
                return null;
            }
            var upgrade = Upgrades.FirstOrDefault(u => u.Id == id);
            ((Player)GameManager.Instance.Player.Entity).UsedSkillPoints++;
            upgrade.NumOfUpgrades++;
            GameManager.Instance.OnUpgraded?.Invoke(this, upgrade);
            return upgrade;
        }
    }
}