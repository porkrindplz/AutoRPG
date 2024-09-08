using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Button RestartButton;
    public void OnEnable()
    {
        RestartButton.onClick.AddListener(RestartGame);
    }
    public void OnDisable()
    {
        RestartButton.onClick.RemoveAllListeners();
    }
    void RestartGame()
    {
        GameManager.Instance.ChangeGameState(EGameState.MainMenu);
        this.gameObject.SetActive(false);
    }
}
