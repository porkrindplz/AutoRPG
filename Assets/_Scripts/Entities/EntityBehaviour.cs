using System;
using _Scripts.Actions.Effects;
using _Scripts.Models;
using UnityEngine;

namespace _Scripts.Entities
{
    public class EntityBehaviour : MonoBehaviour
    {
        [SerializeField] public Entity Entity;
        public event Action OnActiveEffectChanged;
    
        // Start is called before the first frame update
        void Start()
        {
            
        }

        /// <summary>
        /// Adds an active effect to the underlying entity, also sending out an OnActiveEffectChanged event
        /// </summary>
        /// <param name="ae"></param>
        public void AddActiveEffect(ActiveEffect ae)
        {
            Entity.ActiveEffects.Add(ae);
            OnActiveEffectChanged?.Invoke();
        }

        // Update is called once per frame
        void Update()
        {
            if (Entity?.ActiveEffects == null) return;
            foreach (var activeEffect in Entity?.ActiveEffects)
            {
                activeEffect.Update(Time.deltaTime);
            }

            for (int i = Entity.ActiveEffects.Count - 1; i >= 0; i--)
            {
                if (Entity.ActiveEffects[i].IsEffectFinished)
                {
                    Entity.ActiveEffects.RemoveAt(i);
                    OnActiveEffectChanged?.Invoke();
                    
                }
            }
            
        }
    }
}
