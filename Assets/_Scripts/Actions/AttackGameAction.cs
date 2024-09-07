using System;
using _Scripts.Models;

namespace _Scripts.Actions
{
    public class AttackGameAction : IGameAction
    {
        public GameAction GameAction { get; set; }
        
        public double Value { get; set; }


        public void Interact(Entity actor, Entity actee)
        {
            // TODO: make this more interesting
            var totalDamage = Math.Max(actor.GetTotalAttack() - actor.GetTotalDefense(), 0);
            Value = totalDamage;
            actee.CurrentHealth -= totalDamage;
        }
    }
}