using System;
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
        [Serializable]
        private class EnemyJSON
        {
            public List<Enemy> Enemies;
        }
        
        [SerializeField] private GameObject enemyPanel;
        
        private List<Enemy> _allEnemies;
        private Random _random;
        
        public EntityBehaviour CurrentEnemy { get; private set; }

        protected override void Awake()
        {
            // Read JSON into
            LoadEnemyData();
            Debug.Log(_allEnemies.Count);
            _random = new Random();
            base.Awake();
        }
        
        public void SpawnEnemy()
        {
            var newEnemyStats = _allEnemies[_random.Next(0, _allEnemies.Count)].Copy();
            newEnemyStats.OnDeath += OnEnemyDeath;
            var existingEnemy = enemyPanel.GetComponent<EntityBehaviour>();
            if (existingEnemy.Entity != null)
            {
                existingEnemy.Entity.OnDeath -= OnEnemyDeath;    
            }

            CurrentEnemy = existingEnemy;
            
            enemyPanel.GetComponent<EntityBehaviour>().Entity = newEnemyStats;
            var autoAction = enemyPanel.GetComponent<AutoAction>();
            autoAction.weights = newEnemyStats.ActionWeights;
            autoAction.possibleActions = GameManager.Instance.AllActions
                .Where(a => newEnemyStats.Actions.Contains(a.Key))
                .Select(a => a.Value.GameAction).ToList();
            autoAction.PopulateQueue();

            Logger.Log(autoAction.weights.Count.ToString());
            UpdateNextEnemy();
        }

        private void UpdateNextEnemy()
        {
            Logger.Log("Setting next enemy icon");
            
            //
        }

        private void OnEnemyDeath(Entity entity)
        {
            GameManager.Instance.ChangeGameState(EGameState.EnemyDefeated);
        }

        private void LoadEnemyData()
        {
            var enemyData = Resources.LoadAll("Enemies").ToList();
            _allEnemies = new List<Enemy>();
            foreach (EnemyData data in enemyData)
            {
                var enemy = new Enemy()
                {
                     Name = data.name,
                     MaxHealth = data.maxHealth,
                     CurrentHealth = data.maxHealth,
                     MaxMagic = data.maxMagic,
                     CurrentMagic = data.maxMagic,
                     BaseAtk = data.baseAtk,
                     BaseDef = data.baseDef,
                     Speed = data.speed,
                     Nuts = data.baseNuts,
                     Actions = data.actions,
                     ActionWeights = data.actionWeights,
                     Resistances = new Dictionary<ElementsType, float>(),
                     Upgrades = new List<Upgrade>(),
                     SpritePath = ""
                };
                _allEnemies.Add(enemy);
            }
        }
    }
}
