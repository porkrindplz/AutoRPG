using System.Collections;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Utilities;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class ResultTextSpawner : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textPrefab;

        private void OnEnable()
        {
            GameManager.Instance.OnAction += GenerateStatusText;
        }
        private void OnDisable()
        {
            GameManager.Instance.OnAction -= GenerateStatusText;
        }

        void UpdateDisplay(EntityBehaviour entity, string value, Color color)
        {
            var text = Instantiate(textPrefab, transform);

            text.color = color;
            text.text = value;
            text.rectTransform.position = entity.transform.position;
            
            StartCoroutine(AnimateText(text));
        }
    
        void GenerateStatusText(EntityBehaviour actor, EntityBehaviour actee, IGameAction action)
        {
            string text="";
            Color color=Color.white;
        
            switch (action)
            {
                case AttackGameAction damageAction:
                    text = $"-{damageAction.Value}";
                    color = Elements.GetElementColor(damageAction.GameAction.Element);
                    break;
            }
            UpdateDisplay(actee, text, color);
        }
    
        IEnumerator AnimateText(TextMeshProUGUI text)
        {
            Vector2 initScale = text.transform.localScale;
            Vector2 initPosition = text.rectTransform.position;
            float value = 0;
            text.transform.localScale = initScale * value;
        
            while (value < 1)
            {
                value += Time.deltaTime;
                text.transform.localScale = initScale*value;
                text.rectTransform.position = new Vector2(initPosition.x, initPosition.y + value);
                yield return null;
            }
            yield return new WaitForSeconds(.5f);
            Destroy(text.gameObject);
        }
    
    }
}
