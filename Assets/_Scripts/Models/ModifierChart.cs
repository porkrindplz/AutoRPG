using System;

namespace _Scripts.Models
{
    public enum ModiferType
    {
        Neutral = 0,
        Vulnerable,
        Weak,
        Resistant,
        Null,
        Absorb
    }
    
    [Serializable]
    public class ModifierChart
    {
        public ModiferType Melee;
        public ModiferType Ranged;
        public ModiferType AoE;
        public ModiferType Fire;
        public ModiferType Water;
        public ModiferType Leaf;
        public ModiferType Lightning;
        public ModiferType Shadow;

        public static double GetModifier(ModiferType type)
        {
            return type switch
            {
                ModiferType.Neutral => 1,
                ModiferType.Vulnerable => 2,
                ModiferType.Weak => 1.5f,
                ModiferType.Resistant => 0.5f,
                ModiferType.Null => 0,
                ModiferType.Absorb => -0.5f,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}