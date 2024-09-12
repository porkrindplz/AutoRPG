using System.Collections.Generic;
using _Scripts.Actions.Effects;
using _Scripts.Models;
using _Scripts.Utilities;
using UnityEngine;

namespace _Scripts.Entities.EnemyAIs
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] public List<AttackType> possibleActions;
        [SerializeField] public List<double> weights;
        private WeightedRouletteWheel _weighter;

        
        protected EntityBehaviour EnemyBehaviour;

        public void Start()
        {
            _weighter = new WeightedRouletteWheel();
            EnemyBehaviour = GetComponent<EntityBehaviour>();
        }

        public virtual AttackType MakeDecision()
        {
            var attackType = _weighter.SelectItem(possibleActions, weights);
            // Dont cast a shield if you already have one
            if (attackType is AttackType.ShieldFire or AttackType.ShieldLeaf or AttackType.ShieldWater 
                && (EnemyBehaviour.HasActiveEffect(ActiveEffectType.ShieldFire) || EnemyBehaviour.HasActiveEffect(ActiveEffectType.ShieldLeaf) || EnemyBehaviour.HasActiveEffect(ActiveEffectType.ShieldWater)))
            {
                // Try again
                return MakeDecision();
            }
            return attackType;
        }
    }
}