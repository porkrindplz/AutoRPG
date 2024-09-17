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
            var speed = Speed + StatConstants.Instance.SpeedForWhirlwind * GameManager.Instance.AllTrees.Sword.GetUpgradeLevel("whirlwind");
            if (GameManager.Instance.IsAutoBattle)
            {
                speed = (int)(speed * 0.75f);
            }

            return speed;
        }
    }
}