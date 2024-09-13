using System.Collections;
using System.Collections.Generic;
using _Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;
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
    EnemyGroupDefeated,
    PlayerDefeated,
    Walking,
    Paused,
    Story,
    Credits,
}

public class PlayStats
{
    public string PlayerID{get; private set;}
    public int TotalDefeats{get; private set;}
    public int TotalVictories{get; private set;}
    public int TotatStarts{get; private set;}
    public int MostNuts{get; private set; }=0;
    public double TotalTimePlayed{get; private set;}
    
    public PlayStats()
    {
        PlayerID = Guid.NewGuid().ToString();
    }
    public void AddVictory()
    {
        TotalVictories++;
    }
    public void AddDefeat()
    {
        TotalDefeats++;
    }
    public void AddStart()
    {
        TotatStarts++;
    }
    public void UpdateNuts(int nuts)
    {
        if (nuts > MostNuts)
        {
            MostNuts = nuts;
        }
    }
    public void UpdateTimePlayed()
    {
        TotalTimePlayed += Time.realtimeSinceStartupAsDouble;
    }
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

    public Action<UpgradeTree> OnResetTree;

    public Action OnSkillPointGain;
    
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
    public RectTransform StoryPanel;
    [field:SerializeField]public EntityBehaviour Player { get; private set; }
    
    [field:SerializeField]public EGameState CurrentGameState { get; private set; }

    public PlayStats PlayStats { get; private set; }
    
    
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
        if (PlayStats == null)
        {
            PlayStats = new PlayStats();
            TransmitPlayStats();
        }
    }

    public IGameAction GetNewAction(AttackType attackType)
    {
        if (attackType == AttackType.Block) return new BlockAction(){GameAction = AllActions[attackType]};
        IGameAction action = new AttackAction(attackType);
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
            case EGameState.EnemyGroupDefeated:
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
            case EGameState.Story:
                Logger.Log("Story");
                HandleStory();
                break;
            case EGameState.Credits:
                Logger.Log("Credits");
                HandleCredits();
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
        Player.GetComponent<CharacterAnimationController>().EntityImageRect.GetComponent<Image>().enabled = true;

        OnUpgraded?.Invoke(AllTrees.Sword, AllTrees.Sword.Upgrades[0]);

        ScreenFade.Instance.FadeIn(1);
        PlayStats.AddStart();
        TransmitPlayStats();
        //Load Player
        //Load trees
        //Load Enemy
        Logger.Log("Handle Setup");
        AllTrees.Reset();

        StartCoroutine(SetupSequence());
    }

    IEnumerator SetupSequence()
    {
        ScreenFade.Instance.FadeIn(1);
        yield return new WaitForSeconds(1);
        
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
        PlayStats.UpdateNuts(Player.Entity.Nuts);
        PlayStats.UpdateTimePlayed();
        TransmitPlayStats();
        DisableAllInput();
        //Enable play inputs
    }

    private void HandleEnemyDefeated()
    {
        PlayStats.AddVictory();
        PlayStats.UpdateTimePlayed();
        TransmitPlayStats();
        DisableAllInput();
        StartCoroutine(EnemyDefeatedSequence());
    }
    
    IEnumerator EnemyDefeatedSequence()
    {
        yield return new WaitForSeconds(2);
        EnemyManager.Instance.IncrementEnemyIndex();
        VictoryPanel.gameObject.SetActive(true);
    }

    private void HandlePlayerDefeated()
    {
        Player.GetComponent<CharacterAnimationController>().EntityImageRect.GetComponent<Image>().enabled = false;
        DisableAllInput();
        ScreenFade.Instance.FadeIn(1);
        StartCoroutine(PlayerDefeatedSequence());
    }
    IEnumerator PlayerDefeatedSequence()
    {
        yield return new WaitForSeconds(1);
        GameOverPanel.gameObject.SetActive(true);
    }

    private void HandleWalking()
    {
        DisableAllInput();
        //Enable play inputs
    }

    private void HandlePaused()
    {
        PlayStats.UpdateTimePlayed();
        PlayStats.UpdateNuts(Player.Entity.Nuts);
        TransmitPlayStats();
        DisableAllInput();

        //Activate Pause UI
        //Enable pause inputs
    }

    private void HandleStory()
    {
        StoryPanel.gameObject.SetActive(true);
        
    }

    private void HandleCredits()
    {
        
    }

    private void DisableAllInput()
    {
        //Disable all input types
    }
    
    public void TransmitPlayStats()
    {
        Logger.Log($"PlayerID: {PlayStats.PlayerID} \n "+
                   $"Total Victories: {PlayStats.TotalVictories} \n "+
                   $"Total Defeats: {PlayStats.TotalDefeats.ToString()} \n" +
                   $"Total Starts: {PlayStats.TotatStarts.ToString()} \n" +
                   $"Most Nuts: {PlayStats.MostNuts.ToString()} \n" +
                   $"Total Time Played: {PlayStats.TotalTimePlayed.ToString()}");
    }

}
