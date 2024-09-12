using Unity.VisualScripting;

namespace _Scripts.Models
{
    public enum AttackType
    {
        Sword,
        Bow,
        Block,
        BraveSlash,
        CrossSlash,
        Fireball,
        Leaf,
        Lightning,
        MultiShot,
        Shadow,
        Slash,
        SniperShot,
        Water,
        Whirlwind,
        ShieldFire,
        ShieldWater,
        ShieldLeaf,
    }

    public enum AttackGroupType
    {
        Melee,
        Ranged,
        Aoe,
        Magic,
    }

    public static class AttackTypeConverter
    {
        
        public static AttackType? StringToAttackType(string attack)
        {
            attack = attack.Replace(" ", "");
            bool valid = AttackType.TryParse(attack, out AttackType atk);
            return valid ? atk : null;
        }
        
        public static string AttackTypeToString(AttackType type)
        {
            var s = type.ToString().FirstCharacterToLower();
            var newS = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsUpper(s[i]))
                {
                    newS += "_";
                }

                newS += s[i];
            }

            return newS;
        }
    }
}