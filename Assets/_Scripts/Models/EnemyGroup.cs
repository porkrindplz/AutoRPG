using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Models
{
    [Serializable]
    public class EnemyGroup
    {
        public List<string> Enemies;
        
        /// <summary>
        /// Upon defeating the ith enemy, we want to spawn the next enemy. If it is -1 then the group is defeated
        /// </summary>
        [SerializeField] private List<int> NextEnemy;
        public bool ShareHp;

        public int CurrentEnemy;

        public void GoToNextEnemy()
        {
            CurrentEnemy = NextEnemy[CurrentEnemy];
        }

        public string GetCurrentEnemy() => Enemies[CurrentEnemy];

    }
}