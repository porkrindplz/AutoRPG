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

        private IStageChanger _stageChanger;
        
        protected EntityBehaviour EnemyBehaviour;

        public void Start()
        {
            _weighter = new WeightedRouletteWheel();
            EnemyBehaviour = GetComponent<EntityBehaviour>();
        }

        public void AddStageChanger()
        {
            TryGetComponent(out _stageChanger);
        }

        public virtual AttackType MakeDecision()
        {
            return _weighter.SelectItem(possibleActions, weights);
        }
    }
}