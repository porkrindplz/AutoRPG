using System;
using System.Collections.Generic;

namespace _Scripts.Models
{
    [Serializable]
    public class Upgrade
    {
        /// <summary>
        /// Unique Id used to identify the exact upgrade
        /// </summary>
        public string Id;

        public string Name;
        public string Description;

        public int Level;

        /// <summary>
        /// Maximum number of times a player may spec into this upgrade
        /// </summary>
        public int MaxUpgrades;
        public List<Effect> UpgradeEffects  = new List<Effect>();
        
        /// <summary>
        /// Contains the Ids of all upgrades that need to be unlocked before this upgrade can be unlocked
        /// </summary>
        public List<string> Prerequisites  = new List<string>();

        public string PrerequisiteType;
        
        /// <summary>
        /// Once this upgrade has at least one upgrade (NumUpgrades > 0) then all upgrades associated with this
        /// Id can be unlocked assuming the player has enough points
        /// </summary>
        public List<string> Children  = new List<string>();

        public List<string> ExclusiveWith  = new List<string>();
        
        

        /// <summary>
        /// Keeps track of the number of times the player has specced into this upgrade. Cannt be greater than MaxUpgrades
        /// </summary>
        public int NumOfUpgrades;

        public bool CanUpgradeMore() => NumOfUpgrades < MaxUpgrades;
    }
}