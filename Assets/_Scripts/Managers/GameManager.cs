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
using _Scripts.UI;
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
    
    /// <summary>
    /// Emitted anytime that an action is taken by a player. The first entity emits the action, the second
    /// is the target of the action, which may be itself.
    /// </summary>
    public Action<EntityBehaviour, EntityBehaviour, IGameAction> OnAction;

    /// <summary>
    /// Emitted anytime an entity gets an upgrade
    /// </summary>
    public Action<EntityBehaviour, Upgrade> OnUpgraded;
    
    public Action<Entity,int> OnNutsChanged;
    
    public Dictionary<string, IGameAction> AllActions { get; set; }
    public AutoAction AutoAction;
    public EnemyManager EnemyManager;

    public AllTrees AllTrees;

    public int Nuts;
    public RectTransform VictoryPanel;
    
    [field:SerializeField]public EntityBehaviour Player { get; private set; }
    
    [field:SerializeField]public EGameState CurrentGameState { get; private set; }

    protected override void Awake()
    {
        LoadActions();
        LoadUpgradeTrees();
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

    private void LoadUpgradeTrees()
    {
        var text = Resources.Load<TextAsset>("Data/upgrades");
        AllTrees = JsonUtility.FromJson<AllTrees>(text.text);
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EnemyManager.Instance.SpawnEnemy();
        }
    }

    public void ChangeGameState(EGameState newState)
    {
        OnBeforeGameStateChanged?.Invoke(CurrentGameState, newState);
        CurrentGameState = newState;
        switch (newState)
        {
            case EGameState.MainMenu:
                Logger.Log("Main Menu");
                HandleMainMenu();
                break;
            case EGameState.SetupGame:
                HandleSetupGame();
                break;
            case EGameState.SpawnEnemy:
                HandleSpawnEnemy();
                break;
            case EGameState.Playing:
                Logger.Log("Playing");
                HandlePlaying();
                break;
            case EGameState.EnemyDefeated:
                Logger.Log("Enemy Defeated");
                HandleEnemyDefeated();
                break;
            case EGameState.PlayerDefeated:
                Logger.Log("Player Defeated");
                HandlePlayerDefeated();
                break;
            case EGameState.Paused:
                Logger.Log("Paused");
                HandlePaused();
                break;
        }
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
        Logger.Log("Handle Setup");
        ChangeGameState(EGameState.SpawnEnemy);
    }
    private void HandleSpawnEnemy()
    {
        DisableAllInput();
        EnemyManager.Instance.SpawnEnemy();
        ChangeGameState(EGameState.Playing);
    }

    private void HandlePlaying()
    {
        DisableAllInput();
        //Enable play inputs
    }

    private void HandleEnemyDefeated()
    {
        DisableAllInput();

        VictoryPanel.gameObject.SetActive(true);
        
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
