using System;
using _Scripts.Actions;
using _Scripts.Models;
using _Scripts.Utilities;
using UnityEngine;

namespace _Scripts.Entities.EnemyAIs
{
    public interface IStageChanger
    {
        public event Action ChangeStage;
    }
    
    public class TimerStageChanger : MonoBehaviour, IStageChanger
    {
        public CountdownTimer TimerToStageChange;

        public event Action ChangeStage;

        public void Start()
        {
            TimerToStageChange.Start();
        }

        private void Update()
        {
            TimerToStageChange.Tick(Time.deltaTime);
            if (TimerToStageChange.IsFinished)
            {
                ChangeStage?.Invoke();
                Destroy(this);
            }
        }
    }

    public class HealthStageChange : MonoBehaviour, IStageChanger
    {
        public event Action ChangeStage;
        public float healthPercentageToChange;
        private EntityBehaviour _entityBehaviour;


        private void Start()
        {
            _entityBehaviour = GetComponent<EntityBehaviour>();
        }

        private void Update()
        {
            var healthPercentage = (_entityBehaviour.Entity.CurrentHealth / _entityBehaviour.Entity.MaxHealth) * 100;
            if (healthPercentage < healthPercentageToChange)
            {
                ChangeStage?.Invoke();
   
            }
        }
    }

    public class HitStageChange : MonoBehaviour, IStageChanger
    {
        public event Action ChangeStage;
        public int HitsUntilChange;
        public AttackGroupType GroupType;

        private void Start()
        {
            GameManager.Instance.OnAction += OnAction;
        }

        private void OnAction(EntityBehaviour actor, EntityBehaviour actee, IGameAction action)
        {
            if (actee.Entity is Enemy && action.GameAction.AttackGroupType == GroupType)
            {
                HitsUntilChange--;
            }

            if (HitsUntilChange == 0)
            {
                GameManager.Instance.OnAction -= OnAction;
                ChangeStage?.Invoke();
                Destroy(this);
            }
        }
    }
}