using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.Models
{
    [Serializable]
    public class EnemyGroup
    {
        public List<string> Enemies;
        public List<EnemySet> EnemySets;
        /// <summary>
        /// Upon defeating the ith enemy, we want to spawn the next enemy. If it is -1 then the group is defeated
        /// </summary>
        [SerializeField] private List<int> NextEnemy;
        public bool ShareHp;

        public int CurrentEnemy;

        public void GoToNextEnemy()
        {
            if (NextEnemy is null || NextEnemy.Count == 0)
            {
                CurrentEnemy = -1;
                return;
            }
            CurrentEnemy = NextEnemy[CurrentEnemy];
        }

        public string GetCurrentEnemy() => Enemies[CurrentEnemy];

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