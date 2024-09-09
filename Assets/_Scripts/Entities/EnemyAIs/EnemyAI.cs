using System;
using System.Collections.Generic;
using _Scripts.Models;
using _Scripts.Utilities;
using UnityEngine;

namespace _Scripts.Entities
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] public List<AttackType> possibleActions;
        [SerializeField] public List<double> weights;
        private WeightedRouletteWheel _weighter;
        
        protected EntityBehaviour EnemyBehaviour;

        protected List<Stage> Stages;

        public void Start()
        {
            _weighter = new WeightedRouletteWheel();
            EnemyBehaviour = GetComponent<EntityBehaviour>();
        }

        public virtual AttackType MakeDecision()
        {
            return _weighter.SelectItem(possibleActions, weights);
        }
    }
}