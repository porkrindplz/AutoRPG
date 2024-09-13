using System.Collections.Generic;
using _Scripts.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Entities
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemyData", order = 1)]
    public class EnemyData : ScriptableObject
    {
        public string name;
        public double maxHealth;
        public double maxMagic;
        public double baseAtk;
        public double baseDef;
        public double baseMagicAtk;
        public int speed;
        public List<AttackType> actions;
        public List<double> actionWeights;
        public Sprite sprite;
        public ModifierChart Modifiers;

    }
}