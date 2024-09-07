using System.Collections.Generic;
using System.Linq;

namespace _Scripts.Models
{
    public class Entity
    {
        public bool IsPlayer;
        public double CurrentHealth;
        public double MaxHealth;
        public int CurrentMagic;
        public int MaxMagic;
        public double BaseAtk;
        public double BaseDef;
        public int Speed;
        public int MaxSkillPoints;
        public int UsedSkillPoints;

        public Dictionary<ElementsType, float> Resistances { get; set; }
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