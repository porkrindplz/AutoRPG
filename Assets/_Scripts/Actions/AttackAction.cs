using System;
using _Scripts.Entities;
using _Scripts.Models;
using UnityEngine;

namespace _Scripts.Actions
{
    public class AttackAction : IGameAction
    {
        public bool HasStarted { get; set; }
        public event Action OnFinished;
        public GameAction GameAction { get; set; }

        public AttackType AttackType { get; }

        public AttackAction(AttackType type)
        {
            AttackType = type;
        }
        
        public void Interact(EntityBehaviour actor, EntityBehaviour actee)
        {
            var (actorE, acteeE) = (actor.Entity, actee.Entity);
            var dmg = (actorE.BaseAtk*actorE.BaseAtk) / (actorE.BaseAtk + acteeE.BaseDef);

            dmg *= GetDefenseModifier(acteeE);
            
            Debug.Log($"Attack: {AttackType.ToString()} dmg: {dmg}");
            acteeE.CurrentHealth = Math.Max(0, acteeE.CurrentHealth - dmg);
        }

        private double GetDefenseModifier(Entity actee)
        {
            Debug.Log(AttackType);
            return AttackType switch
            {
                AttackType.Fireball => ModifierChart.GetModifier(actee.ReceivedModifiers.Fire),
                AttackType.Leaf => ModifierChart.GetModifier(actee.ReceivedModifiers.Leaf),
                AttackType.Water => ModifierChart.GetModifier(actee.ReceivedModifiers.Water),
                AttackType.Lightning => ModifierChart.GetModifier(actee.ReceivedModifiers.Lightning),
                AttackType.Shadow => ModifierChart.GetModifier(actee.ReceivedModifiers.Fire),
                AttackType.Sword => ModifierChart.GetModifier(actee.ReceivedModifiers.Melee),
                AttackType.Block => 1,
                AttackType.BraveSlash => ModifierChart.GetModifier(actee.ReceivedModifiers.AoE),
                AttackType.CrossSlash => ModifierChart.GetModifier(actee.ReceivedModifiers.Melee),
                AttackType.Slash => ModifierChart.GetModifier(actee.ReceivedModifiers.Melee),
                AttackType.Whirlwind => ModifierChart.GetModifier(actee.ReceivedModifiers.AoE),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Update(float deltaTime)
        {
            
        }
    }
}