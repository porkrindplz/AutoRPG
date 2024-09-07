using System.Collections;
using System.Collections.Generic;
using _Scripts.Utilities;
using UnityEngine;
using System;
using System.Linq;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Managers;
using _Scripts.Models;
using Logger = _Scripts.Utilities.Logger;


public enum EGameState
{
    MainMenu,
    SetupGame,
    SpawnEnemy,
    Playing,
    EnemyDefeated,
    PlayerDefeated,
    Walking,
    Paused,
}
public class GameManager : Singleton<GameManager>
{
    public event Action<EGameState,EGameState> OnBeforeGameStateChanged;
    public event Action<EGameState,EGameState> OnAfterGameStateChanged;
    
    /// <summary>
    /// Emitted anytime that an action is taken by a player. The first entity emits the action, the second
    /// is the target of the action, which may be itself.
    /// </summary>
    public Action<EntityBehaviour, EntityBehaviour, IGameAction> OnAction;

    /// <summary>
    /// Emitted anytime an entity gets an upgrade
    /// </summary>
    public Action<EntityBehaviour, Upgrade> OnUpgraded;
    
    public Dictionary<string, IGameAction> AllActions { get; set; }
    public AutoAction AutoAction;
    public EnemyManager EnemyManager;
    
    
    public UpgradeTree WarriorTree;
    public UpgradeTree WizardTree;
    public EGameState CurrentGameState { get; private set; }

    protected override void Awake()
    {
        LoadActions();
        base.Awake();
    }

    protected void Start()
    {
        ChangeGameState(EGameState.SetupGame);
    }

    private void LoadActions()
    {
        AllActions = new Dictionary<string, IGameAction>()
        {
            { "attack", new AttackGameAction() },
            { "block", new BlockGA() }
        };
        var allActions = Resources.LoadAll<GameAction>("Actions").ToList();
        foreach (var action in AllActions)
        {
            AllActions[action.Key].GameAction = allActions.FirstOrDefault(a => a.Name == action.Key);
        }
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            EnemyManager.Instance.SpawnEnemy();
        }
    }

    public void ChangeGameState(EGameState newState)
    {
        OnBeforeGameStateChanged?.Invoke(CurrentGameState, newState);
        
        switch (newState)
        {
            case EGameState.MainMenu:
                Logger.Log("Main Menu");
                break;
            case EGameState.SetupGame:
                Logger.Log("Setup Game");
                break;
            case EGameState.Playing:
                Logger.Log("Playing");
                break;
            case EGameState.EnemyDefeated:
                Logger.Log("Enemy Defeated");
                break;
            case EGameState.PlayerDefeated:
                Logger.Log("Player Defeated");
                break;
            case EGameState.Paused:
                Logger.Log("Paused");
                break;
        }
        OnAfterGameStateChanged?.Invoke(CurrentGameState, newState);

        CurrentGameState = newState;
    }

    private void HandleMainMenu()
    {
        DisableAllInput();
        //Enable Main Menu Inputs
    }

    private void HandleSetupGame()
    {
        //Load Player
        //Load trees
        //Load Enemy
    }
    private void HandleSpawnEnemy()
    {
        DisableAllInput();
        EnemyManager.SpawnEnemy();
    }

    private void HandlePlaying()
    {
        DisableAllInput();
        AutoAction.enabled = true;

        //Enable play inputs
    }

    private void HandleEnemyDefeated()
    {
        DisableAllInput();

        //Enable Reward UI & Inputs
        
    }

    private void HandlePlayerDefeated()
    {
        DisableAllInput();
        
        //Enable Gameover UI & Inputs
    }

    private void HandleWalking()
    {
        DisableAllInput();
        //Enable play inputs
    }

    private void HandlePaused()
    {
        DisableAllInput();

        //Activate Pause UI
        //Enable pause inputs
    }

    private void DisableAllInput()
    {
        //Disable all input types
    }

}
