using _Scripts.Entities;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class EffectsUIController : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        private TextMeshProUGUI text;
        
        // Start is called before the first frame update
        void OnEnable()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();

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
                text.text = "";
                return;
            }
            var s = "Effects:\n";
            foreach (var effect in entityBehaviour.Entity.ActiveEffects)
            {
                s += effect.ActiveEffectType + "\n";
            }

            text.text = s;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
