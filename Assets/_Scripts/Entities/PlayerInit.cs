using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Entities;
using _Scripts.Models;
using UnityEngine;

public class PlayerInit : MonoBehaviour
{
    private EntityBehaviour playerEntity;
    
    CharacterAnimationController AnimationController;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        playerEntity = GetComponent<EntityBehaviour>();
        AnimationController = GetComponent<CharacterAnimationController>();
        GameManager.Instance.OnBeforeGameStateChanged += (_, gameState) =>
        {
            if (gameState == EGameState.SetupGame)
            {
                playerEntity.Entity = new Player
                {
                    CurrentHealth = 100,
                    MaxHealth = 100,
                    CurrentMagic = 25,
                    MaxMagic = 25,
                    BaseAtk = 10,
                    BaseDef = 2,
                    Speed = 1,
                    ReceivedModifiers = new ModifierChart(),
                    Upgrades = new List<Upgrade>(),
                    MaxSkillPoints = 3,
                    UsedSkillPoints = 0,

                };
                playerEntity.Entity.OnDeath += _ =>
                {
                    AnimationController.DeathAnimation(playerEntity.Entity);
                    GameManager.Instance.ChangeGameState(EGameState.PlayerDefeated);
                };
            }

        };

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
