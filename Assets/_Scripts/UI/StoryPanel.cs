using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StoryPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private Button nextButton;
    [SerializeField] private RawImage image;
    
    [SerializeField]private StorySO ActiveStory;
    
    private int pageIndex=0;
    
    
    void Awake(){
    }

    private void OnEnable()
    {
        SetStory();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(EndStory());
        }
    }

    public void SetStory()
    {
        ActiveStory = StoryManager.Instance.ActiveStory;
        pageIndex = 0;
        StartCoroutine(StartStory());
    }
    
    IEnumerator StartStory()
    {
        UpdateStory();
        ScreenFade.Instance.FadeIn(1);
        yield return new WaitForSeconds(1);
    }
    
    public void NextPage()
    {
        pageIndex++;
        
        if (pageIndex >= ActiveStory.Pages.Length)
        {
            StartCoroutine(EndStory());
            return;
        }
        
        StartCoroutine(NextPageAnimation());
    }
    
    IEnumerator NextPageAnimation()
    {
        //fade out text and image then next page then fade in text and image. do not fade panel
        float duration = 0.5f;
        
        float timer = 0;
        while (timer < duration)
        {
            float alpha = Mathf.Lerp(1, 0, timer / duration);
            storyText.color = new Color(storyText.color.r, storyText.color.g, storyText.color.b, alpha);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        
        UpdateStory();
        
        timer = 0;
        while (timer < duration)
        {
            float alpha = Mathf.Lerp(0, 1, timer / duration);
            storyText.color = new Color(storyText.color.r, storyText.color.g, storyText.color.b, alpha);
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        
    }
        
    private void UpdateStory()
    {
        storyText.text = ActiveStory.Pages[pageIndex].text;
        if (ActiveStory.Pages[pageIndex].image != null)
        {
            image.texture = ActiveStory.Pages[pageIndex].image;
        }
    }
    
    private IEnumerator EndStory()
    {
        ScreenFade.Instance.FadeOut(1, Color.black);
        yield return new WaitForSeconds(1);
        GameManager.Instance.ChangeGameState(ActiveStory.NextState);
        gameObject.SetActive(false);
    }

}
