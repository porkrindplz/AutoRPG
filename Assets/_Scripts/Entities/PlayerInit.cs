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
                    BaseDef = 5,
                    Speed = 50,
                    ReceivedModifiers = new ModifierChart(),
                    Upgrades = new List<Upgrade>(),
                    MaxSkillPoints = 2,
                    UsedSkillPoints = 0,

                };
                playerEntity.Entity.OnDeath += _ =>
                {
                    AnimationController.DeathAnimation(playerEntity.Entity);
                    GameManager.Instance.ChangeGameState(EGameState.PlayerDefeated);
                };
            }
        };

        GameManager.Instance.OnUpgraded += (tree, upgrade) =>
        {
            switch (upgrade.Id)
            {
                case "resist_fire":
                    playerEntity.Entity.ReceivedModifiers.Fire = ModifierType.Resistant;
                    playerEntity.Entity.ReceivedModifiers.Water = ModifierType.Weak;
                    break;
                case "resist_water":
                    playerEntity.Entity.ReceivedModifiers.Water = ModifierType.Resistant;
                    playerEntity.Entity.ReceivedModifiers.Leaf = ModifierType.Weak;
                    break;
                case "resist_leaf":
                    playerEntity.Entity.ReceivedModifiers.Leaf = ModifierType.Resistant;
                    playerEntity.Entity.ReceivedModifiers.Fire = ModifierType.Weak;
                    break;
                case "resist_physical":
                    playerEntity.Entity.ReceivedModifiers.Melee = ModifierType.Resistant;
                    break;
                case "resist_ranged":
                    playerEntity.Entity.ReceivedModifiers.Ranged = ModifierType.Resistant;
                    break;
            }
        };

        GameManager.Instance.OnResetTree += tree =>
        {
            if (tree.Name != "Shield") return;
            playerEntity.Entity.ReceivedModifiers.Water = ModifierType.Neutral;
            playerEntity.Entity.ReceivedModifiers.Fire = ModifierType.Neutral;
            playerEntity.Entity.ReceivedModifiers.Water = ModifierType.Neutral;
        };

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
