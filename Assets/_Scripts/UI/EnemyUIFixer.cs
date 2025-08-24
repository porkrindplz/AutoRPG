using UnityEngine;
using UnityEngine.UI;

public class EnemyUIFixer : MonoBehaviour
{
    private Image enemyImage;
    private Sprite defaultSprite;
    
    void Start()
    {
        enemyImage = GetComponent<Image>();
    }

    public void SetEnemySprite(Sprite sprite)
    {
        defaultSprite = sprite;
    }
    
    void Update()
    {
        // Constantly ensure the sprite doesn't get overridden
        if (enemyImage.sprite != defaultSprite)
        {
            enemyImage.sprite = defaultSprite;
        }
    }
    

    void LateUpdate()
    {
        if (enemyImage.sprite != defaultSprite)
        {
            enemyImage.sprite = defaultSprite;
        }
    }

}