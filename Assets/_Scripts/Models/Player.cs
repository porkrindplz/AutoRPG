using _Scripts.Managers;

namespace _Scripts.Models
{
    public class Player : Entity
    {
        public int MaxSkillPoints;
        public int UsedSkillPoints;

        public override double GetDefense()
        {
            return BaseDef + StatConstants.Instance.DefenseForBraveSlash * GameManager.Instance.AllTrees.Sword.GetUpgradeLevel("brave_slash");
        }

        public override double GetSpeed()
        {
            return Speed + StatConstants.Instance.SpeedForWhirlwind * GameManager.Instance.AllTrees.Sword.GetUpgradeLevel("whirlwind");
        }
    }
}