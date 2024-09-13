using System;
using System.Linq;
using _Scripts.Actions;
using _Scripts.Actions.Effects;
using _Scripts.Managers;
using _Scripts.Models;
using _Scripts.Utilities;
using UnityEngine;

namespace _Scripts.Entities.EnemyAIs
{
    public class SnakeAI : EnemyAI
    {

        public CountdownTimer timerToShieldBreak;
        public AttackType shieldType;
        private bool hasShieldInit = false;
        private bool hasShieldBroken = false;
        
        
        public override AttackType MakeDecision()
        {
                        
            if (hasShieldInit && EnemyBehaviour.Entity.ActiveEffects.Count == 0)
            {
                hasShieldBroken = true;
            } 
            
            if (hasShieldBroken)
            {
                EnemyManager.Instance.OnStageChange();
                Destroy(this);
                return AttackType.None;
            }

            
            if (hasShieldInit == false && timerToShieldBreak.IsFinished)
            {
                timerToShieldBreak.Stop();
                hasShieldInit = true;
                return shieldType;
            }

            return base.MakeDecision();
        }

        public void Update()
        {

            if (timerToShieldBreak is null)
            {
                timerToShieldBreak = new CountdownTimer(15);
                timerToShieldBreak.Start();
            }
            
            timerToShieldBreak.Tick(Time.deltaTime);
            
            /*if (EnemyBehaviour.HasActiveEffect(ActiveEffectType.ShieldFire) ||
                EnemyBehaviour.HasActiveEffect(ActiveEffectType.ShieldWater) ||
                EnemyBehaviour.HasActiveEffect(ActiveEffectType.ShieldLeaf))
            {
                
            }*/
            
// Shield has been broken, change state
                       
        }

    }
}