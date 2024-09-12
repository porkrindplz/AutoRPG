using System;
using System.Collections;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace _Scripts.UI
{
    public class ResultTextSpawner : MonoBehaviour
    {
        
        [SerializeField] private TextMeshProUGUI textPrefab;
        [SerializeField] private float shiftDistance;
        private Canvas parentCanvas;
        
        
        private void Awake()
        {
            parentCanvas = FindObjectOfType<Canvas>();
        }

        private void OnEnable()
        {
            GameManager.Instance.OnAction += GenerateStatusText;
        }
        private void OnDisable()
        {
            if(GameManager.Instance!=null)
                GameManager.Instance.OnAction -= GenerateStatusText;
        }
    
        void UpdateDisplay(EntityBehaviour entity, string value, Color color)
        {
            var text = Instantiate(textPrefab, parentCanvas.transform);

            text.color = color;
            text.text = value;
            text.rectTransform.position = entity.GetComponent<CharacterAnimationController>().EntityImageRect.position +(Vector3.up*shiftDistance);
            
            StartCoroutine(AnimateText(text));
        }
    
        void GenerateStatusText(EntityBehaviour actor, EntityBehaviour actee, IGameAction action)
        {
            string text="";
            Color color=Color.white;
            if (action is AttackAction attackAction)
            {
                var mod = attackAction.GetModifier(actee.Entity);
                text = mod switch
                {
                    -0.5 => "Absorb: ",
                    0.0 => "Null: ",
                    0.5 => "Resist: ",
                    1.5 => "Weak: ",
                    2.0 => "Vulnerable: ",
                    > 2 => "Rekt: ",
                    _ => ""
                };
            }
        
            switch (action)
            {
                case AttackAction damageAction:
                    text += $"-{damageAction.Value}";
                    color = Elements.GetElementColor(damageAction.GameAction.Element);
                    break;
                case BlockAction blockAction:
                    text = $"Blocking";
                    color = Elements.GetElementColor(blockAction.GameAction.Element);
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
                text.transform.localScale = initScale*(value + 0.5f);
                text.transform.position = new Vector2(initPosition.x, initPosition.y + value*shiftDistance);
                if(value>0.75f)
                    text.color = new Color(text.color.r, text.color.g, text.color.b, (1 - value)/ (1 - .75f));
                yield return null;
            }
            yield return new WaitForSeconds(.5f);
            Destroy(text.gameObject);
        }
    
    }
}
