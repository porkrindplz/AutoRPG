using System;
using _Scripts.Utilities;
using UnityEngine;

namespace _Scripts.Managers
{

    public class StatConstants : Singleton<StatConstants>
    {
        [Header("Sword Constants")]
        public float SwordMultiplier = 0.25f;
        public float SlashMultiplier = 0.2f;
        public float CrossSlashMultiplier = 0.2f;

        [Header("Shield Constants")] 
        public float ShieldTime = 5; 

        [Header("Magic Constants")]
        public float CannonModifier = 0.15f;
        public float MagicModifier = 0.2f;

        [Header("Slingshot Constants")]
        public float BowModifier = 0.25f;
        public float SniperShotModifier = 0.2f;
    }
}