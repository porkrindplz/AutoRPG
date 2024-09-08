using System;
using _Scripts.Models;
using UnityEngine;

namespace _Scripts.Actions
{
    public class AttackGameAction : IGameAction
    {
        public GameAction GameAction { get; set; }
        
        public double Value { get; set; }


        public void Interact(Entity actor, Entity actee)
        {
            // TODO: make this more interesting
            Debug.Log("Attack");
            var totalDamage = Math.Max(actor.BaseAtk - actor.BaseDef, 0);
            Value = totalDamage;
            actee.CurrentHealth -= totalDamage;
        }
    }
}