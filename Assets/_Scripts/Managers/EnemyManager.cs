using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Entities;
using _Scripts.Models;
using Unity.VisualScripting;
using UnityEngine;
using Logger = _Scripts.Utilities.Logger;
using Random = System.Random;

namespace _Scripts.Managers
{
    
    public class EnemyManager : Utilities.Singleton<EnemyManager>
    {
        [Serializable]
        private class EnemyJSON
        {
            public List<Enemy> Enemies;
        }
        
        [SerializeField] private GameObject enemyPanel;

        [SerializeField] private List<string> enemyOrder;
        private List<Enemy> _allEnemies;
        private Random _random;

        private int EnemyIndex;
        
        public EntityBehaviour CurrentEnemy { get; private set; }

        protected override void Awake()
        {
            // Read JSON into
            LoadEnemyData();
            Debug.Log(_allEnemies.Count);
            _random = new Random();
            GameManager.Instance.OnBeforeGameStateChanged += (state, gameState) =>
            {
                if (gameState == EGameState.SetupGame)
                {
                    EnemyIndex = 0;
                }
            };
            base.Awake();
        }
        
        public void SpawnEnemy()
        {
            var newEnemyStats = _allEnemies.Find(e => e.Name == enemyOrder[EnemyIndex]).Copy();
            newEnemyStats.OnDeath += OnEnemyDeath;
            var existingEnemy = enemyPanel.GetComponent<EntityBehaviour>();
            if (existingEnemy.Entity != null)
            {
                existingEnemy.Entity.OnDeath -= OnEnemyDeath;    
            }

            CurrentEnemy = existingEnemy;
            
            enemyPanel.GetComponent<EntityBehaviour>().Entity = newEnemyStats;
            enemyPanel.GetComponent<CharacterAnimationController>().EntityImage.sprite = newEnemyStats.Sprite;
            var enemyAi = enemyPanel.GetComponent<EnemyAI>();
            
            enemyAi.weights = newEnemyStats.ActionWeights;
            enemyAi.possibleActions = newEnemyStats.Actions;
            EnemyIndex++;
            //autoAction.PopulateQueue();

            Logger.Log(enemyAi.weights.Count.ToString());
            UpdateNextEnemy();
        }

        private void UpdateNextEnemy()
        {
            Logger.Log("Setting next enemy icon");
            
            //
        }

        private void OnEnemyDeath(Entity entity)
        {
            GameManager.Instance.EnemyNuts = entity.Nuts;
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
                     ReceivedModifiers = data.Modifiers,
                     Upgrades = new List<Upgrade>(),
                     Sprite = data.sprite
                };
                _allEnemies.Add(enemy);
            }
        }
    }
}
