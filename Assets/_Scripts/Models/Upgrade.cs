using System.Collections.Generic;

namespace _Scripts.Models
{
    public class Upgrade
    {
        /// <summary>
        /// Unique Id used to identify the exact upgrade
        /// </summary>
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        /// <summary>
        /// Maximum number of times a player may spec into this upgrade
        /// </summary>
        public int MaxUpgrades { get; set; }
        public Effect UpgradeEffect { get; set; } = new Effect();
        
        /// <summary>
        /// Contains the Ids of all upgrades that need to be unlocked before this upgrade can be unlocked
        /// </summary>
        public List<string> Prerequisites { get; set; } = new List<string>();
        
        /// <summary>
        /// Once this upgrade has at least one upgrade (NumUpgrades > 0) then all upgrades associated with this
        /// Id can be unlocked assuming the player has enough points
        /// </summary>
        public List<string> Children { get; set; } = new List<string>();

        /// <summary>
        /// Keeps track of the number of times the player has specced into this upgrade. Cannt be greater than MaxUpgrades
        /// </summary>
        public int NumOfUpgrades;

        public bool CanUpgradeMore() => NumOfUpgrades < MaxUpgrades;
    }
}