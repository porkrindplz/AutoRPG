using System.Collections.Generic;
using System.Linq;

namespace _Scripts.Models
{

    public class UpgradeTree
    {
        public List<Upgrade> Upgrades { get; set; } = new List<Upgrade>();

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

    public class Effect
    {
        public string Attribute { get; set; }
        public double Value { get; set; }
        public string Operation { get; set; }
    }
}