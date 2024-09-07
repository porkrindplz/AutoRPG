using _Scripts.Models;
using UnityEngine;

namespace _Scripts.Utilities
{
    public static class Elements
    {
        
        public static Color GetElementColor(ElementsType element)
        {
            switch (element)
            {
                case ElementsType.Fire:
                    return Color.red;
                case ElementsType.Water:
                    return Color.blue;
                case ElementsType.Earth:
                    return Color.green;
                case ElementsType.Ice:
                    return Color.cyan;
                case ElementsType.Electric:
                    return Color.yellow;
                case ElementsType.Poison:
                    return Color.magenta;
                default:
                    return Color.white;
            }
        }
    }
}