using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Actions.Effects;
using Unity.Mathematics;
using UnityEngine;

namespace _Scripts.Models
{
    public class Entity
    {
        public event Action<Entity> OnDeath;

        public double CurrentHealth
        {
            get => currentHealth;
            set
            {
                currentHealth = value;
                if (currentHealth <= 0)
                {
                    OnDeath?.Invoke(this);
                }
            }
        }

        private double currentHealth;
        public double MaxHealth;
        public double CurrentMagic;
        public double MaxMagic;
        public double BaseAtk;
        public double BaseDef;
        [Range(0, 100)]
        public int Speed;
        
        public float GetSpeedMultiplier() => (float)(0.5 + (float)(100-Speed) / 100 * (2 - 0.5));

        /// <summary>
        /// List of all active effects being applied, these are temporary and should affect stats on Getters.
        /// DO NOT add effects through here, add them through Entity Behaviour. This is bad design process
        /// </summary>
        public List<ActiveEffect> ActiveEffects { get; set; } = new();

        public int Nuts
        {
            get => nuts;
            set
            {
                nuts = value;
                GameManager.Instance.OnNutsChanged?.Invoke(this, Nuts);
            }
        }
        private int nuts;

        /// <summary>
        /// Modifiers affect how much damage you take from certain attack
        /// </summary>
        public ModifierChart ReceivedModifiers;
        //public Dictionary<ElementsType, float> ElementalBonus { get; set; }

        public List<Upgrade> Upgrades = new();
        
        public double GetDefense()
        {
            double multipliers = 1;
            if (ActiveEffects.FindIndex(ae => ae.ActiveEffectType == ActiveEffectType.Block) != -1)
            {
                multipliers *= 2f;
            }

            return BaseDef * multipliers;
        }
        
        //ublic double GetTotalAttack() => GetTotal(BaseAtk, "Attack");
        //public double GetTotalDefense() => GetTotal(BaseDef, "Defense");

        /*public double GetStatAdditions(string upgradeAttribute)
        {
            var bonuses = Upgrades.Where(u => u.UpgradeEffect.Attribute == upgradeAttribute);
            return bonuses.Where(ab => ab.UpgradeEffect.Operation == "add")
                .Sum(ab => ab.UpgradeEffect.Value);
        }
        
        public double GetMultiplier(string upgradeAttribute)
        {
            var bonuses = Upgrades.Where(u => u.UpgradeEffect.Attribute == upgradeAttribute);
            if (!bonuses.Any()) return 1;
            var multipliers = bonuses.Where(ab => ab.UpgradeEffect.Operation == "multiply")
                .Select(ab => ab.UpgradeEffect.Value)
                .Aggregate((a, s) => a*s);
            return multipliers;
        }
        
        private double GetTotal(double baseStat, string upgradeAttribute)
        {
            baseStat += GetStatAdditions(upgradeAttribute);
            var multipliers = GetMultiplier(upgradeAttribute);
            return baseStat * multipliers;
        }*/
    }
}