using System.Collections;
using System.Collections.Generic;
using _Scripts.Utilities;
using UnityEngine;
using System;
using Logger = _Scripts.Utilities.Logger;


public enum EGameState
{
    MainMenu,
    SetupGame,
    Playing,
    EnemyDefeated,
    PlayerDefeated,
    Paused,
}
public class GameManager : Singleton<GameManager>
{
    public event Action<EGameState,EGameState> OnBeforeGameStateChanged;
    public event Action<EGameState,EGameState> OnAfterGameStateChanged;
    public EGameState CurrentGameState { get; private set; }
    
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

    private void HandlePlaying()
    {
        DisableAllInput();

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
