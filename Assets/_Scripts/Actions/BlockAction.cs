using System;
using _Scripts.Actions.Effects;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.Models;
using _Scripts.Utilities;
using UnityEngine;

namespace _Scripts.Actions
{
    public class BlockAction : IGameAction
    {
        public bool HasStarted { get; set; }
        public event Action OnFinished;
        public GameAction GameAction { get; set; }
        private CountdownTimer _activeTimer;
        
        public void Interact(EntityBehaviour actor, EntityBehaviour actee)
        {
            Debug.Log("Block");
            _activeTimer = new CountdownTimer(StatConstants.Instance.ShieldTime);
            actee.AddActiveEffect(new ActiveEffect(ActiveEffectType.Block, StatConstants.Instance.ShieldTime));
            HasStarted = true;
            _activeTimer.Start();
        }

        public void Update(float deltaTime)
        {
            _activeTimer.Tick(deltaTime);
            if (_activeTimer.IsFinished)
            {
                HasStarted = false;
                OnFinished?.Invoke();
                
            }
        }
    }
}