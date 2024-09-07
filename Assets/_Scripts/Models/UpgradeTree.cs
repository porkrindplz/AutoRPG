using System;
using System.Collections.Generic;
using System.Linq;

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
        public bool CanUpgrade(List<Upgrade> currentUpgrades, string id)
        {
            var newUpgrade = Upgrades.FirstOrDefault(u => u.Id == id);
            if (newUpgrade is null) return false;

            if (!newUpgrade.CanUpgradeMore()) return false;

            foreach (var prereq in newUpgrade.Prerequisites)
            {
                if (currentUpgrades.FindIndex(u => u.Id == prereq) == -1)
                {
                    return false;
                }
            }
            return true;
        }
    }

    [Serializable]
    public class Effect
    {
        public string Attribute;
        public double Value;
        public string Operation;
    }
}