using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Models
{
    [Serializable]
    public class EnemyGroup
    {
        //public List<string> Enemies;
        public List<EnemySet> EnemySets;
        /// <summary>
        /// Upon defeating the ith enemy, we want to spawn the next enemy. If it is -1 then the group is defeated
        /// </summary>
        //[SerializeField] private List<int> NextEnemy;
        public bool ShareHp;

        [NonSerialized] public int CurrentEnemy;
        
        /// <summary>
        /// Determines if you should get an additional skill point after being the enemy group
        /// </summary>
        public bool SkillPointRewarded;
        
        /// <summary>
        /// Minimum number of nuts achieved for winning this battle group
        /// </summary>
        public int MinNutsWon;

        /// <summary>
        /// Time in seconds it takes for the nut count to go from 1.5*MinNutsWon to 1.0*MinNutsWon
        /// </summary>
        public int TimeForNutLoss;
        
        /// <summary>
        /// An amount that starts at 1.5x MinNutsWon, and slowly decrements as time progresses down to MinNutsWon
        /// </summary>
        ///
        //
        [NonSerialized] public float ActualNutsWon;

        public void GoToNextEnemy()
        {
            CurrentEnemy = EnemySets[CurrentEnemy].NextEnemy;
        }

        public string GetCurrentEnemy() => EnemySets[CurrentEnemy].EnemyName;
        public int GetCurrentSetCount() => EnemySets.Count;
        public int GetGroupCount() => EnemySets.Count;
        public EnemySet GetCurrentEnemySet() => EnemySets[CurrentEnemy];
    }

    public enum EnemyStateChangeType
    {
        None,
        Timer,
        Health,
        Death,
        Hit
    }

    [Serializable]
    public class EnemySet
    {
        public int NextEnemy;
        public string EnemyName;
        public EnemyStateChangeType StateChangeType;
        public float Timer;
        public float HealthPercentage;
        public int HitCount;
        public AttackGroupType HitGroupType;
    }
}