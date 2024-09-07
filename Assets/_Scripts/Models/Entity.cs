using System;
using System.Collections.Generic;
using System.Linq;

namespace _Scripts.Models
{
    public class Entity
    {
        public double CurrentHealth;
        public double MaxHealth;
        public double CurrentMagic;
        public double MaxMagic;
        public double BaseAtk;
        public double BaseDef;
        public int Speed;

        public Dictionary<ElementsType, float> Resistances = new();
        //public Dictionary<ElementsType, float> ElementalBonus { get; set; }

        public List<Upgrade> Upgrades = new();

        public double GetTotalAttack() => GetTotal(BaseAtk, "Attack");
        public double GetTotalDefense() => GetTotal(BaseDef, "Defense");

        public double GetStatAdditions(string upgradeAttribute)
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
        }
    }
}