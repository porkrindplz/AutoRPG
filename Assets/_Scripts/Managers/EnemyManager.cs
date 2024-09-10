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
        public event System.Action<Enemy> OnEnemySpawned;
        
        
        [Serializable]
        private class EnemyJSON
        {
            public List<Enemy> Enemies;
        }
        
        private GameObject enemyPanel;

        [SerializeField] private List<EnemyGroup> enemyOrder;
        private List<Enemy> _allEnemies;
        private Random _random;

        private int EnemyIndex;
        
        
        public EntityBehaviour CurrentEnemy { get; private set; }

        protected override void Awake()
        {
            // Read JSON into
            enemyPanel = GameObject.Find("EnemyPanel");
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

        public void IncrementEnemyIndex() => EnemyIndex++;
        
        public void SpawnEnemy()
        {
            var nextEnemy = enemyOrder[EnemyIndex].GetCurrentEnemy();
            var newEnemyStats = _allEnemies.Find(e => e.Name == nextEnemy).Copy();
            
            newEnemyStats.OnDeath += OnEnemyDeath;
            var existingEnemy = enemyPanel.GetComponent<EntityBehaviour>();
            if (existingEnemy.Entity != null)
            {
                existingEnemy.Entity.OnDeath -= OnEnemyDeath;    
            }
            
            // Carry over hp 
            if (enemyOrder[EnemyIndex].ShareHp && existingEnemy.Entity != null)
            {
                newEnemyStats.CurrentHealth = existingEnemy.Entity.CurrentHealth;
                newEnemyStats.CurrentMagic = existingEnemy.Entity.CurrentMagic;
            }

            CurrentEnemy = existingEnemy;
            OnEnemySpawned?.Invoke(newEnemyStats);
            
            enemyPanel.GetComponent<EntityBehaviour>().Entity = newEnemyStats;
            enemyPanel.GetComponent<CharacterAnimationController>().EntityImage.sprite = newEnemyStats.Sprite;
            var enemyAi = enemyPanel.GetComponent<EnemyAI>();
            
            enemyAi.weights = newEnemyStats.ActionWeights;
            enemyAi.possibleActions = newEnemyStats.Actions;
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
            CurrentEnemy.GetComponent<CharacterAnimationController>().DeathAnimation(entity);
            GameManager.Instance.EnemyNuts = entity.Nuts;
            
            // Go to next enemy, if -1 then we have defeated enemy
            
            enemyOrder[EnemyIndex].GoToNextEnemy();
            if (enemyOrder[EnemyIndex].CurrentEnemy == -1)
            {
                
                GameManager.Instance.ChangeGameState(EGameState.EnemyGroupDefeated);    
            }
            else
            {
                SpawnEnemy();
            }
            
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
