using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
     [SerializeField] private Button MainMenuButton;
    [SerializeField] private Button TryAgainButton;
    public void OnEnable()
    {
        MainMenuButton.onClick.AddListener(RestartGame);
        TryAgainButton.onClick.AddListener(TryAgain);
    }
    public void OnDisable()
    {
        MainMenuButton.onClick.RemoveAllListeners();
        TryAgainButton.onClick.RemoveAllListeners();
    }
    
    
    void RestartGame()
    {
        GameManager.Instance.ChangeGameState(EGameState.MainMenu);
        this.gameObject.SetActive(false);
    }

    void TryAgain()
    {
        GameManager.Instance.ChangeGameState(EGameState.Retry);
        this.gameObject.SetActive(false);
    }
}
