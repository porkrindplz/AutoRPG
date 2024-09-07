using System;
using _Scripts.Models;

namespace _Scripts.Actions
{
    public class AttackGameAction : IGameAction
    {
        public void Interact(Entity actor, Entity actee)
        {
            // TODO: make this more interesting
            var totalDamage = Math.Max(actor.GetTotalAttack() - actor.GetTotalDefense(), 0);
            actee.CurrentHealth -= totalDamage;
        }
    }
}