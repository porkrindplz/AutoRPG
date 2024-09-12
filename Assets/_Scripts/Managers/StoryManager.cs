using System.Collections;
using System.Collections.Generic;
using _Scripts.Utilities;
using UnityEngine;
using Logger = _Scripts.Utilities.Logger;

public class StoryManager : Singleton<StoryManager>
{
    [field: SerializeField] public StorySO[] Stories;
    
    public StorySO ActiveStory { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    public void SetStory(StoryType type)
    {
        Logger.Log("Setting story" + type);
        for(int i = 0; i < Stories.Length; i++)
        {
            if(Stories[i].Type == type)
            {
                Logger.Log("Story Set: " + type);
                ActiveStory= Stories[i];
                Logger.Log("-------------STORY SET-------------" + ActiveStory);
                break;
            }
        }
    }
}
