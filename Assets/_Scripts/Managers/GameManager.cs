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
    public Action<UpgradeTree, Upgrade> OnUpgraded;
    
    public Dictionary<AttackType, GameAction> AllActions { get; set; }
    
    /// <summary>
    ///  Emitted anytime deez nuts are updated
    /// </summary>
    public Action<Entity,int> OnNutsChanged;
    
    public AutoAction AutoAction;
    public EnemyManager EnemyManager;

    public AllTrees AllTrees;
    
    [Header("UI")]
    public RectTransform MainMenuPanel;
    public RectTransform VictoryPanel;
    public RectTransform GameOverPanel;
    [field:SerializeField]public EntityBehaviour Player { get; private set; }
    
    [field:SerializeField]public EGameState CurrentGameState { get; private set; }

    public int EnemyNuts
    {
        get => enemyNuts;
        set
        {
            enemyNuts = value;
        }
    }
    private int enemyNuts;
    
    
    protected override void Awake()
    {
        LoadActions();
        LoadUpgradeTrees();
        base.Awake();
    }

    public IGameAction GetNewAction(AttackType attackType)
    {
        IGameAction action = attackType switch
        {
            AttackType.Sword => new AttackGameAction(),
            AttackType.Block => new BlockAction(),
            AttackType.Fireball => new AttackAction(AttackType.Fireball),
            AttackType.Water => new AttackAction(AttackType.Water),
            AttackType.Leaf => new AttackAction(AttackType.Leaf),
            AttackType.Lightning => new AttackAction(AttackType.Lightning),
            AttackType.Shadow => new AttackAction(AttackType.Shadow),
            _ => throw new Exception($"Invalid action {attackType}")
        };
        action.GameAction = AllActions[attackType];
        return action;
    }
    
    protected void Start()
    {
        ChangeGameState(EGameState.MainMenu);
    }

    private void LoadActions()
    {
        AllActions = new Dictionary<AttackType, GameAction>();
        var allActions = Resources.LoadAll<GameAction>("Actions").ToList();
        foreach (var action in allActions)
        {
            AllActions[action.Name] = allActions.FirstOrDefault(a => a.Name == action.Name);
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
        MainMenuPanel.gameObject.SetActive(true);
    }

    private void HandleSetupGame()
    {
        
        //Load Player
        //Load trees
        //Load Enemy
        Logger.Log("Handle Setup");
        AllTrees.Reset();
        OnUpgraded?.Invoke(AllTrees.Sword, AllTrees.Sword.Upgrades[0]);
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

        StartCoroutine(EnemyDefeatedSequence());
    }
    
    IEnumerator EnemyDefeatedSequence()
    {
        yield return new WaitForSeconds(2);
        VictoryPanel.gameObject.SetActive(true);
    }

    private void HandlePlayerDefeated()
    {
        DisableAllInput();

        StartCoroutine(PlayerDefeatedSequence());
    }
    IEnumerator PlayerDefeatedSequence()
    {
        yield return new WaitForSeconds(2);
        GameOverPanel.gameObject.SetActive(true);
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
