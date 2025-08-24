using UnityEngine;
using UnityEngine.UI;

public class MaintainSpriteStateBehaviour : StateMachineBehaviour
{
    private Image enemyImage;
    private Sprite originalSprite;
    

    public void SetSprite(Sprite sprite)
    {
        originalSprite = sprite;
    }
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyImage == null)
        {
            enemyImage = animator.GetComponent<Image>();
            originalSprite = enemyImage.sprite;
        }
        
        // Always reset to the original sprite when entering any state
        enemyImage.sprite = originalSprite;
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Continuously ensure the sprite is maintained
        if (enemyImage.sprite != originalSprite)
        {
            enemyImage.sprite = originalSprite;
        }
    }
}