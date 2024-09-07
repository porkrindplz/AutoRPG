using System.Collections.Generic;
using System.Linq;
using _Scripts.Entities;
using _Scripts.Models;
using _Scripts.Utilities;
using UnityEngine;
using Logger = _Scripts.Utilities.Logger;
using Random = System.Random;

namespace _Scripts.Managers
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        [SerializeField] private GameObject enemyPrefab;
        
        private List<Enemy> _allEnemies;
        private Random _random;

        protected override void Awake()
        {
            // Read JSON into
            _allEnemies = new List<Enemy>();
            _random = new Random();
            base.Awake();
        }
        
        public void SpawnEnemy()
        {
            var newEnemyStats = _allEnemies[_random.Next(0, _allEnemies.Count)];
            var newEnemy = Instantiate(enemyPrefab);
            newEnemy.GetComponent<EntityBehaviour>().Entity = newEnemyStats;
            var autoAction = newEnemy.GetComponent<AutoAction>();
            autoAction.weights = newEnemyStats.ActionWeights;
            autoAction.possibleActions = GameManager.Instance.AllActions
                .Where(a => newEnemyStats.Actions.Contains(a.Key))
                .Select(a => a.Value.GameAction).ToList();
            UpdateNextEnemy();
        }

        private void UpdateNextEnemy()
        {
            Logger.Log("Setting next enemy icon");
            
            //
        }
    }
}
