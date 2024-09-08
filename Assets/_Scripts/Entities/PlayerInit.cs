using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Entities;
using _Scripts.Models;
using UnityEngine;

public class PlayerInit : MonoBehaviour
{
    private EntityBehaviour playerEntity;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        playerEntity = GetComponent<EntityBehaviour>();
        GameManager.Instance.OnBeforeGameStateChanged += (_, gameState) =>
        {
            if (gameState == EGameState.SetupGame)
            {
                playerEntity.Entity = new Player
                {
                    CurrentHealth = 10,
                    MaxHealth = 10,
                    CurrentMagic = 25,
                    MaxMagic = 25,
                    BaseAtk = 10,
                    BaseDef = 5,
                    Speed = 1,
                    Resistances = new Dictionary<ElementsType, float>(),
                    Upgrades = new List<Upgrade>(),
                    MaxSkillPoints = 3,
                    UsedSkillPoints = 0,

                };
                playerEntity.Entity.OnDeath += _ =>
                {
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
