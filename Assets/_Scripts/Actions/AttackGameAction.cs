using System;
using _Scripts.Entities;
using _Scripts.Models;
using UnityEngine;

namespace _Scripts.Actions
{
    public class AttackGameAction : IGameAction
    {
        public bool HasStarted { get; set; }
        public event Action OnFinished;
        public GameAction GameAction { get; set; }
        
        public double Value { get; set; }


        public void Interact(EntityBehaviour actor, EntityBehaviour actee)
        {
            // TODO: make this more interesting
            Debug.Log("Attack");
            HasStarted = true;
            var totalDamage = Math.Max(actor.Entity.BaseAtk - actee.Entity.GetDefense(), 0);
            Value = totalDamage;
            actee.Entity.CurrentHealth -= totalDamage;
            OnFinished?.Invoke();
        }

        public void Update(float deltaTime)
        {
            
        }
    }
}