using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace _Scripts.Models
{
    public class Enemy : Entity
    {
        public string Name;
        public List<AttackType> Actions;
        public List<double> ActionWeights;
        public string SpritePath;

        /// <summary>
        /// Creates a shallow copy of the object. NOTE: if we are modifying stuff like resistances or actions in
        /// each enemy we need to refactor
        /// </summary>
        /// <returns></returns>
        public Enemy Copy()
        {
            return new Enemy
            {
                CurrentHealth = CurrentHealth,
                MaxHealth = MaxHealth,
                CurrentMagic = CurrentMagic,
                MaxMagic = MaxMagic,
                BaseAtk = BaseAtk,
                BaseDef = BaseDef,
                Speed = Speed,
                Nuts = Nuts,
                Modifiers = Modifiers,
                Upgrades = Upgrades,
                Name = Name,
                Actions = Actions,
                ActionWeights = ActionWeights,
                SpritePath = SpritePath,
            };
        }
    }
}