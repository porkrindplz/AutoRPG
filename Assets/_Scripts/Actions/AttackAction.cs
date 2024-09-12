using System;
using _Scripts.Actions.Effects;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.Models;
using UnityEngine;
using Logger = _Scripts.Utilities.Logger;

namespace _Scripts.Actions
{
    public class AttackAction : IGameAction
    {
        public bool HasStarted { get; set; }
        public event Action OnFinished;
        public GameAction GameAction { get; set; }

        public AttackType AttackType { get; }
        
        public double Value { get; set; }

        public AttackAction(AttackType type)
        {
            AttackType = type;
        }
        
        public void Interact(EntityBehaviour actor, EntityBehaviour actee)
        {
            
            var (actorE, acteeE) = (actor.Entity, actee.Entity);
            var dmg = (actorE.BaseAtk*actorE.BaseAtk) / (actorE.BaseAtk + acteeE.BaseDef);

            HandleEnemyShieldCast(actor);
            dmg *= CheckShieldBreaks(actee);

            // Elemental shields at this point soak up all damage
            if (actee.HasActiveEffect(ActiveEffectType.ShieldFire) ||
                actee.HasActiveEffect(ActiveEffectType.ShieldLeaf) ||
                actee.HasActiveEffect(ActiveEffectType.ShieldWater))
            {
                return;
            }


            var upgradeModifiers = GetUpgradeModifier(actorE, acteeE);
            dmg *= upgradeModifiers;
            
            dmg *= GetModifier(acteeE);

            dmg = Math.Round(dmg, 1);
            
            Debug.Log($"Attack: {AttackType.ToString()} dmg: {dmg}");
            Value = dmg;
            acteeE.CurrentHealth = Math.Max(0, acteeE.CurrentHealth - dmg);
            OnFinished?.Invoke();
        }

        /// <summary>
        /// Checks to see if an elemental shield can be broken. Player must have enchant + the correct effective element to break
        /// </summary>
        private double CheckShieldBreaks(EntityBehaviour actee)
        {
            // Check for enchant modifier for shield breaks
            if (GameAction.AttackGroupType != AttackGroupType.Melee ||
                GameManager.Instance.AllTrees.Staff.GetUpgradeLevel("enchant") <= 0) return 1;
            
            Debug.Log("Elemental attack, attempting to shield break");
            if (GameManager.Instance.AllTrees.Staff.GetUpgradeLevel("fireball") > 0 &&
                actee.HasActiveEffect(ActiveEffectType.ShieldLeaf))
            {
                Logger.Log("Leaf shield broken by fire!");
                actee.RemoveActiveEffect(ActiveEffectType.ShieldLeaf);
                return 0;
            }
            
            if (GameManager.Instance.AllTrees.Staff.GetUpgradeLevel("leaf") > 0 &&
                actee.HasActiveEffect(ActiveEffectType.ShieldWater))
            {
                Logger.Log("Water shield broken by leaf!");
                actee.RemoveActiveEffect(ActiveEffectType.ShieldWater);
                return 0;
            }
            
            if (GameManager.Instance.AllTrees.Staff.GetUpgradeLevel("water") > 0 &&
                actee.HasActiveEffect(ActiveEffectType.ShieldFire))
            {
                Logger.Log("Fire shield broken by water!");
                actee.RemoveActiveEffect(ActiveEffectType.ShieldFire);
                return 0;
            }
            return 1;
        }

        private void HandleEnemyShieldCast(EntityBehaviour actor)
        {
            if (AttackType == AttackType.ShieldFire) actor.Entity.ActiveEffects.Add(new ActiveEffect(ActiveEffectType.ShieldFire, 100000));
            if (AttackType == AttackType.ShieldLeaf) actor.Entity.ActiveEffects.Add(new ActiveEffect(ActiveEffectType.ShieldLeaf, 100000));
            if (AttackType == AttackType.ShieldWater) actor.Entity.ActiveEffects.Add(new ActiveEffect(ActiveEffectType.ShieldWater, 100000));
        }

        private double GetUpgradeModifier(Entity actor, Entity actee)
        {
            if (actor is Enemy) return 1;

            switch (GameAction.AttackGroupType)
            {
                case AttackGroupType.Melee:
                {
                    var swordLevel = GameManager.Instance.AllTrees.Sword.GetUpgradeLevel("sword");
                    var bonusSwordMult = 1 + swordLevel * StatConstants.Instance.SwordMultiplier;

                    var slashLevel = GameManager.Instance.AllTrees.Sword.GetUpgradeLevel("slash");
                    bonusSwordMult += slashLevel * StatConstants.Instance.SlashMultiplier;
                    // TODO: upgrade slash should increase speed of melee

                    var crossSlashLevel = GameManager.Instance.AllTrees.Sword.GetUpgradeLevel("cross_slash");
                    bonusSwordMult += crossSlashLevel * StatConstants.Instance.CrossSlashMultiplier;

                    return bonusSwordMult;
                }
                // Check for AoE or Cannon modifiers
                case AttackGroupType.Magic:
                {
                    double bonusMagicDmg = 1;

                    bonusMagicDmg +=
                        GameManager.Instance.AllTrees.Staff.GetUpgradeLevel(
                            AttackTypeConverter.AttackTypeToString(AttackType)) * StatConstants.Instance.MagicModifier;
                
                
                    if (GameManager.Instance.AllTrees.Staff.GetUpgradeLevel("aoe") > 0)
                    {
                        // TODO: make this more involved with # of enemies
                        return ModifierChart.GetModifier(actee.ReceivedModifiers.Aoe);
                    }

                    var cannonLevel = GameManager.Instance.AllTrees.Staff.GetUpgradeLevel("cannon");
                    if (cannonLevel > 0)
                    {
                        bonusMagicDmg += cannonLevel * StatConstants.Instance.CannonModifier;
                    }

                    return bonusMagicDmg;
                }
                case AttackGroupType.Ranged:
                {
                    double bonusRangedDmg = 1;
                    bonusRangedDmg +=
                        GameManager.Instance.AllTrees.Slingshot.GetUpgradeLevel("bow") * StatConstants.Instance.BowModifier;

                    bonusRangedDmg += GameManager.Instance.AllTrees.Slingshot.GetUpgradeLevel("sniper_shot") *
                                      StatConstants.Instance.SniperShotModifier;
                    
                    if (GameManager.Instance.AllTrees.Staff.GetUpgradeLevel("aoe") > 0)
                    {
                        // TODO: make this more involved with # of enemies
                        bonusRangedDmg *= ModifierChart.GetModifier(actee.ReceivedModifiers.Aoe);
                    }
                    
                    return bonusRangedDmg;
                }
                default:
                    return 1;
            }
        }

        public double GetModifier(Entity actee)
        {
            Debug.Log(AttackType);
            return AttackType switch
            {
                AttackType.Fireball => ModifierChart.GetModifier(actee.ReceivedModifiers.Fire),
                AttackType.Leaf => ModifierChart.GetModifier(actee.ReceivedModifiers.Leaf),
                AttackType.Water => ModifierChart.GetModifier(actee.ReceivedModifiers.Water),
                AttackType.Lightning => ModifierChart.GetModifier(actee.ReceivedModifiers.Lightning),
                AttackType.Shadow => ModifierChart.GetModifier(actee.ReceivedModifiers.Shadow),
                AttackType.Sword => ModifierChart.GetModifier(actee.ReceivedModifiers.Melee),
                AttackType.Bow => ModifierChart.GetModifier(actee.ReceivedModifiers.Ranged),
                AttackType.SniperShot => ModifierChart.GetModifier(actee.ReceivedModifiers.Ranged),
                AttackType.MultiShot => ModifierChart.GetModifier(actee.ReceivedModifiers.Ranged) * ModifierChart.GetModifier(actee.ReceivedModifiers.Aoe),
                AttackType.Block => 1,
                AttackType.BraveSlash => ModifierChart.GetModifier(actee.ReceivedModifiers.Melee),
                AttackType.CrossSlash => ModifierChart.GetModifier(actee.ReceivedModifiers.Aoe),
                AttackType.Slash => ModifierChart.GetModifier(actee.ReceivedModifiers.Melee),
                AttackType.Whirlwind => ModifierChart.GetModifier(actee.ReceivedModifiers.Aoe),
                
                _ => 0
            };
        }

        public void Update(float deltaTime)
        {
            
        }
    }
}