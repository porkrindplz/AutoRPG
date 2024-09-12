using System;
using UnityEngine.Serialization;

namespace _Scripts.Models
{
    public enum ModifierType
    {
        Neutral = 0,
        Vulnerable,
        Weak,
        Resistant,
        Null,
        Absorb,
        Empty
    }
    
    [Serializable]
    public class ModifierChart
    {
        public ModifierType Melee;
        public ModifierType Ranged;
        [FormerlySerializedAs("AoE")] public ModifierType Aoe;
        public ModifierType Fire;
        public ModifierType Water;
        public ModifierType Leaf;
        public ModifierType Lightning;
        public ModifierType Shadow;

        public static double GetModifier(ModifierType type)
        {
            return type switch
            {
                ModifierType.Neutral => 1,
                ModifierType.Vulnerable => 2,
                ModifierType.Weak => 1.5f,
                ModifierType.Resistant => 0.5f,
                ModifierType.Null => 0,
                ModifierType.Absorb => -0.5f,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}