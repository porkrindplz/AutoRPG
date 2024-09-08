using System;
using System.Collections.Generic;
using System.Linq;
using __Scripts.Systems;

namespace _Scripts.Models
{

    [Serializable]
    public class AllTrees
    {
        public UpgradeTree Warrior;
        public UpgradeTree Paladin;
    }
    
    [Serializable]
    public class UpgradeTree
    {
        public List<Upgrade> Upgrades;

        /// <summary>
        /// Checks if the given hero's upgrades allows for the new upgrade to be chosen.
        /// Criteria is based on if the skill tree contains the given id, all its prereqs have been researched,
        /// and there are still more possible upgrades that could be chosen
        /// </summary>
        public bool CanUpgrade( string id)
        {
            var currentUpgrades = Upgrades.Where(u => u.NumOfUpgrades > 0).ToList();
            var newUpgrade = Upgrades.FirstOrDefault(u => u.Id == id);
            if (newUpgrade is null) return false;

            if (!newUpgrade.CanUpgradeMore()) return false;

            foreach (var prereq in newUpgrade.Prerequisites)
            {
                if (currentUpgrades.FindIndex(u=>u.Id ==prereq)==-1)
                {
                    return false;
                }
            }
            foreach(var exclusive in newUpgrade.ExclusiveWith)
            {
                if (currentUpgrades.FindIndex(u=>u.Id ==exclusive)!=-1)
                {
                    return false;
                }
            }
            return true;
        }
        
        public void TryUpgrade(string id)
        {
            if (!CanUpgrade(id))
            {
                AudioSystem.Instance.PlayNegativeSound();
                return;
            }
            AudioSystem.Instance.PlayMenuConfirmSound();
            var upgrade = Upgrades.FirstOrDefault(u => u.Id == id);
            upgrade.NumOfUpgrades++;
        }
    }
}