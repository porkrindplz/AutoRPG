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
            if (gameState == EGameState.MainMenu)
            {
                playerEntity.Entity = new Player
                {
                    CurrentHealth = 1000,
                    MaxHealth = 1000,
                    CurrentMagic = 25,
                    MaxMagic = 25,
                    BaseAtk = 10,
                    BaseMagicAtk = 7,
                    BaseDef = 5,
                    Speed = 65,
                    ReceivedModifiers = new ModifierChart(),
                    Upgrades = new List<Upgrade>(),
                    MaxSkillPoints = 3,
                    UsedSkillPoints = 0,

                };
                playerEntity.Entity.OnDeath -= OnEntityOnDeath;
                playerEntity.Entity.OnDeath += OnEntityOnDeath;
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
            if (tree.Name == "Shield")
            {
                playerEntity.Entity.ReceivedModifiers.Water = ModifierType.Neutral;
                playerEntity.Entity.ReceivedModifiers.Fire = ModifierType.Neutral;
                playerEntity.Entity.ReceivedModifiers.Leaf = ModifierType.Neutral;
                playerEntity.Entity.ReceivedModifiers.Melee = ModifierType.Neutral;
                playerEntity.Entity.ReceivedModifiers.Ranged = ModifierType.Neutral;
                playerEntity.Entity.ReceivedModifiers.Aoe = ModifierType.Neutral;
            }
        };

    }

    private void OnEntityOnDeath(Entity _)
    {
        GameManager.Instance.PlayStats.AddDefeat();
        GameManager.Instance.PlayStats.UpdateTimePlayed();
        GameManager.Instance.TransmitPlayStats();

        AnimationController.DeathAnimation(playerEntity.Entity);
        StoryManager.Instance.SetStory(StoryType.GameOver);
        GameManager.Instance.ChangeGameState(EGameState.Story);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
