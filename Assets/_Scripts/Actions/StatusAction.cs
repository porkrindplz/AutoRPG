using System;
using _Scripts.Actions.Effects;
using _Scripts.Entities;
using _Scripts.Models;

namespace _Scripts.Actions
{
    public class StatusAction : IGameAction
    {
        public bool HasStarted { get; set; }
        public event Action OnFinished;
        public GameAction GameAction { get; set; }
        public void Interact(EntityBehaviour actor, EntityBehaviour actee)
        {
            if (GameAction.Name == AttackType.Honey && !actee.HasActiveEffect(ActiveEffectType.Honey))
            {
                actee.AddActiveEffect(new ActiveEffect(ActiveEffectType.Honey, 7.5f));
            }

            if (GameAction.Name == AttackType.Smoke && !actee.HasActiveEffect(ActiveEffectType.Smoke))
            {
                actee.AddActiveEffect(new ActiveEffect(ActiveEffectType.Smoke, 7.5f));
            }
            OnFinished?.Invoke();
        }

        public void Update(float deltaTime)
        {
            
        }
    }
}