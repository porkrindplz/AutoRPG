using System.Collections;
using System.Collections.Generic;
using __Scripts.Systems;
using _Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
[SerializeField] private Button StartButton;
    public void OnEnable()
    {
        StartButton.onClick.AddListener(StartGame);
        ScreenFade.Instance.FadeIn(2);

    }
    public void OnDisable()
    {
        StartButton.onClick.RemoveAllListeners();
    }
    
    public void PlayConfirmSound()
    {
        AudioSystem.Instance.PlayMenuConfirmSound();
    }
    
    void StartGame()
    {
        ScreenFade.Instance.FadeOut(.5f, Color.black);
        StartCoroutine(StartGameCoroutine());
    }
    IEnumerator StartGameCoroutine()
    {
        StoryManager.Instance.SetStory(StoryType.Intro);
    
        yield return new WaitForSeconds(.5f);
        GameManager.Instance.ChangeGameState(EGameState.Story);
        gameObject.SetActive(false);
    }
    
}
