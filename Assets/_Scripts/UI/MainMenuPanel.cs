using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
[SerializeField] private Button StartButton;
    public void OnEnable()
    {
        StartButton.onClick.AddListener(StartGame);
    }
    public void OnDisable()
    {
        StartButton.onClick.RemoveAllListeners();
    }
    void StartGame()
    {
        GameManager.Instance.ChangeGameState(EGameState.SetupGame);
        this.gameObject.SetActive(false);
    }
}
