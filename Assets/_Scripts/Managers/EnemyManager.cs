using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Entities;
using _Scripts.Entities.EnemyAIs;
using _Scripts.Models;
using _Scripts.Utilities;
using Unity.VisualScripting;
using UnityEngine.UI;
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

        [SerializeField] private float RatioOfStartingToMinNuts = 1.5f;
        [SerializeField] private List<EnemyGroup> enemyOrder;
        private List<Enemy> _allEnemies;
        private Random _random;

        private int EnemyIndex;
        
        
        public EntityBehaviour CurrentEnemy { get; private set; }

        protected override void Awake()
        {
            // Read JSON into
            enemyPanel = GameObject.Find("EnemyPanel");
            foreach (var group in enemyOrder)
            {
                group.ActualNutsWon = (int)(RatioOfStartingToMinNuts * group.MinNutsWon);
            }
            LoadEnemyData();
            Debug.Log(_allEnemies.Count);
            _random = new Random();
            GameManager.Instance.OnBeforeGameStateChanged += (state, gameState) =>
            {
                if (gameState == EGameState.MainMenu)
                {
                    EnemyIndex = 0;
                }

                if (gameState == EGameState.PlayerDefeated)
                {
                    GetCurrentGroup().CurrentEnemy = 0;
                }
                
            };
            base.Awake();
        }
        
        public EnemyGroup GetCurrentGroup() => enemyOrder[EnemyIndex];
        public int GetEnemiesLeft() => enemyOrder[EnemyIndex].GetGroupCount()-enemyOrder[EnemyIndex].CurrentEnemy;
        
        public void IncrementEnemyIndex()
        {

            if (enemyOrder[EnemyIndex].SkillPointRewarded)
            {
                ((Player)GameManager.Instance.Player.Entity).MaxSkillPoints++;
                GameManager.Instance.OnSkillPointGain?.Invoke();
            }

            EnemyIndex++;
            if (EnemyIndex >= enemyOrder.Count())
            {
                GameManager.Instance.ChangeGameState(EGameState.Story);
                StoryManager.Instance.SetStory(StoryType.Ending);
            }
        }

        public void SpawnEnemy()
        {
            var nextEnemy = enemyOrder[EnemyIndex].GetCurrentEnemySet();
            var newEnemyStats = _allEnemies.Find(e => e.Name == nextEnemy.EnemyName).Copy();
            
            GameManager.Instance.OnNutsChanged?.Invoke(newEnemyStats, (int)GetCurrentGroup().ActualNutsWon);
            
            newEnemyStats.OnDeath += OnEnemyDeath;
            var existingEnemy = enemyPanel.GetComponent<EntityBehaviour>();
            if (existingEnemy.Entity != null)
            {
                existingEnemy.Entity.OnDeath -= OnEnemyDeath;
                if (existingEnemy.TryGetComponent<IStageChanger>(out var changer))
                {
                    changer.ChangeStage -= OnStageChange;
                }
                
            }
            

            switch (nextEnemy.StateChangeType)
            {
                case EnemyStateChangeType.Death:
                    break;
                case EnemyStateChangeType.None:
                    break;
                case EnemyStateChangeType.Timer:
                    var timerStageChanger = existingEnemy.AddComponent<TimerStageChanger>();
                    timerStageChanger.TimerToStageChange = new CountdownTimer(nextEnemy.Timer);
                    timerStageChanger.ChangeStage += OnStageChange;
                    break;
                case EnemyStateChangeType.Health:
                    var healthStageChanger = existingEnemy.AddComponent<HealthStageChange>();
                    healthStageChanger.healthPercentageToChange = nextEnemy.HealthPercentage;
                    healthStageChanger.ChangeStage += OnStageChange;
                    break;
                case EnemyStateChangeType.Hit:
                    var hitStageChanger = existingEnemy.AddComponent<HitStageChange>();
                    hitStageChanger.HitsUntilChange = nextEnemy.HitCount;
                    hitStageChanger.GroupType = nextEnemy.HitGroupType;
                    hitStageChanger.ChangeStage += OnStageChange;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // Carry over hp 
            if (enemyOrder[EnemyIndex].ShareHp && existingEnemy.Entity != null)
            {
                newEnemyStats.CurrentHealth = existingEnemy.Entity.CurrentHealth;
                newEnemyStats.CurrentMagic = existingEnemy.Entity.CurrentMagic;
            }
            CurrentEnemy?.RemoveAllEffects();

            CurrentEnemy = existingEnemy;
            
            
            enemyPanel.GetComponent<EntityBehaviour>().Entity = newEnemyStats;
            enemyPanel.GetComponent<CharacterAnimationController>().EntityImageRect.GetComponent<Image>().sprite = newEnemyStats.Sprite;
            
            //DestroyImmediate(enemyPanel.GetComponent<EnemyAI>());


            
            // Snake AI
            /*if (newEnemyStats.Name == "Water Snake")
            {
                enemyPanel.AddComponent<SnakeAI>();
                enemyPanel.GetComponent<SnakeAI>().shieldType = AttackType.ShieldWater;
            }
            else if (newEnemyStats.Name == "Fire Snake")
            {
                enemyPanel.AddComponent<SnakeAI>();
                enemyPanel.GetComponent<SnakeAI>().shieldType = AttackType.ShieldFire;
            }
            else if (newEnemyStats.Name == "Leaf Snake")
            {
                enemyPanel.AddComponent<SnakeAI>();
                enemyPanel.GetComponent<SnakeAI>().shieldType = AttackType.ShieldLeaf;
            }*/
            //else
            //{
               // enemyPanel.AddComponent<EnemyAI>();
            //}

            EnemyAI enemyAi = enemyPanel.GetComponent<EnemyAI>();




            enemyAi.weights = newEnemyStats.ActionWeights;
            enemyAi.possibleActions = newEnemyStats.Actions;
            //autoAction.PopulateQueue();
            
            OnEnemySpawned?.Invoke(newEnemyStats);
            
            // If there is an action to execute right away, then do it
            if (newEnemyStats.FirstAction != AttackType.None)
            {
                var autoAct = enemyPanel.GetComponent<AutoAction>();
                autoAct.ActionQueue.Clear();
                autoAct.AddAction(newEnemyStats.FirstAction);
            }

            Logger.Log(enemyAi.weights.Count.ToString());
            UpdateNextEnemy();
        }

        private void UpdateNextEnemy()
        {
            Logger.Log("Setting next enemy icon");
            
            //
        }

        public void OnStageChange()
        {
            Logger.Log("Changing stage");
            enemyOrder[EnemyIndex].GoToNextEnemy();
            SpawnEnemy();
        }

        private void OnEnemyDeath(Entity entity)
        { 
            CurrentEnemy.GetComponent<CharacterAnimationController>().DeathAnimation(entity);
            
            // Go to next enemy, if -1 then we have defeated enemy
            var currentNuts = enemyOrder[EnemyIndex].ActualNutsWon;
            
            enemyOrder[EnemyIndex].GoToNextEnemy();
            if (enemyOrder[EnemyIndex].CurrentEnemy == -1)
            {
                GameManager.Instance.EnemyNuts = (int)currentNuts;    
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
                     BaseMagicAtk = data.baseMagicAtk,
                     Speed = data.speed,
                     FirstAction = data.startingAction,
                     //Nuts = data.baseNuts,
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
