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
            var dmg = (actorE.BaseAtk*actorE.BaseAtk) / (actorE.BaseAtk + actorE.BaseDef);

            if (acteeE.Modifiers.ContainsKey(AttackType))
            {
                dmg *= actorE.Modifiers[AttackType];
            }
            
            Debug.Log($"Elemental Attack: {AttackType.ToString()} dmg: {dmg}");
            
        }

        public void Update(float deltaTime)
        {
            
        }
    }
}