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

        public int CurrentEnemy;
        
        /// <summary>
        /// Determines if you should get an additional skill point after being the enemy group
        /// </summary>
        public bool SkillPointRewarded;
        
        /// <summary>
        /// Time in seconds where you get an S ranking for beating the enemy that speed. Nuts will also decay in some proportion to this value
        /// </summary>
        public double GoalBestTime;

        public void GoToNextEnemy()
        {
            CurrentEnemy = EnemySets[CurrentEnemy].NextEnemy;
        }

        public string GetCurrentEnemy() => EnemySets[CurrentEnemy].EnemyName;
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