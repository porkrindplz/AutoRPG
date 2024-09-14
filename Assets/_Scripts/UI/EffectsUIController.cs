using _Scripts.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class EffectsUIController : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private Image shieldImage;
        
        // Start is called before the first frame update
        void OnEnable()
        {
            
            shieldImage.enabled = false;

            GameManager.Instance.OnBeforeGameStateChanged += (state, gameState) =>
            {
                if (gameState == EGameState.Playing)
                {
                    entityBehaviour.OnActiveEffectChanged += OnActiveEffectChanged;
                }
                else
                {
                    entityBehaviour.OnActiveEffectChanged -= OnActiveEffectChanged;
                }
            };
        }

        private void OnActiveEffectChanged()
        {
            if(entityBehaviour.Entity.ActiveEffects.Count == 0)
            {
                shieldImage.enabled = false;
                return;
            }

            shieldImage.enabled = true;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
