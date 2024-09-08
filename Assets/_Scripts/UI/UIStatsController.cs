using System;
using System.Linq;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Models;
using TMPro;
using UnityEngine;
using ElementsType = _Scripts.Models.ElementsType;

namespace _Scripts.UI
{
    public class UIStatsController : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entity;
        [SerializeField] private TextMeshProUGUI statText;
        [SerializeField] private TextMeshProUGUI resistanceText;
        [SerializeField] private bool isPlayer;
    
        // Start is called before the first frame update
        void Start()
        {
            GameManager.Instance.OnUpgraded += OnUpgraded;
            GameManager.Instance.OnAction += OnAction;
        }

        private void OnAction(EntityBehaviour actor, EntityBehaviour actee, IGameAction action)
        {
            if (entity.GetType() == actor.GetType())
            {
                UpdateStats(actor);
            }

            if (entity.GetType() == actee.GetType())
            {
                UpdateStats(actee);
            }
        }

        private void OnDestroy()
        {
        }

        void OnUpgraded(Upgrade upgrade)
        {
            UpdateStats(entity);
        }

        void UpdateStats(EntityBehaviour entityBehaviour)
        {
            if ((entityBehaviour.Entity is Player && isPlayer == false) ||
                (entityBehaviour.Entity is Enemy && isPlayer)) return;
            
            statText.text = $"Health: {(int)entityBehaviour.Entity.CurrentHealth}/{(int)entityBehaviour.Entity.MaxHealth}\n" +
                            $"Magic: {(int)entityBehaviour.Entity.CurrentMagic}/{(int)entityBehaviour.Entity.MaxMagic}\n" +
                            $"Attack: {(int)entityBehaviour.Entity.BaseAtk}\n" +
                            $"Defense: {(int)entityBehaviour.Entity.BaseDef}\n" +
                            $"Speed: {entityBehaviour.Entity.Speed}";
            var resText = "Resistances\n";
            /*foreach (var element in Enum.GetValues(typeof(ElementsType)).Cast<ElementsType>())
            {
                resText += $"{element.ToString()}: {entityBehaviour.Entity.Resistances}\n";
            }*/

            resistanceText.text = resText;
        }


    }
}
