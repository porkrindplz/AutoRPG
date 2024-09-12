
using UnityEngine;

public enum StoryType
{
    Intro,
    GameOver,
    Ending,
    Tutorial1,
    Tutorial2,
    Tutorial3,
    Tutorial4,
}
[System.Serializable]
public class Story
{
    public StoryType type;
    public StoryPage[] Pages;
    public EGameState nextState;
}
[System.Serializable]
public class StoryPage{
    [TextArea(3,10)]
    public string text;
    public Texture image;
}
[CreateAssetMenu(fileName = "Story", menuName = "Story")]
public class StorySO : ScriptableObject
{
    [field:SerializeField]public StoryType Type {get; private set;}
    [field:SerializeField]public StoryPage[] Pages {get; private set;}
    [field:SerializeField]public EGameState NextState {get; private set;}
}
