using System;
using System.Linq;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Models;
using TMPro;
using UnityEngine;
using ElementsType = _Scripts.Models.ElementsType;
using Logger = _Scripts.Utilities.Logger;

namespace _Scripts.UI
{
    public class UIStatsController : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entity;
        [SerializeField] private TextMeshProUGUI statText;
        [SerializeField] private TextMeshProUGUI resistanceText;
        [SerializeField] private TextMeshProUGUI nutsText;
        [SerializeField] private bool isPlayer;
        

        // Start is called before the first frame update
        void OnEnable()
        {
            GameManager.Instance.OnUpgraded += OnUpgraded;
            GameManager.Instance.OnAction += OnAction;
            GameManager.Instance.OnNutsChanged += OnNutsChanged;
            GameManager.Instance.OnBeforeGameStateChanged += OnStateChange;
        }
        private void OnStateChange(EGameState before, EGameState after)
        {
            if (after == EGameState.Playing)
            {
                UpdateStats(entity);
            }
            if(!isPlayer && after == EGameState.EnemyDefeated)
            {
                Logger.Log("Clear!");
                ClearStats();
            }
        }

        private void OnAction(EntityBehaviour actor, EntityBehaviour actee, IGameAction action)
        {
            Debug.Log(action.GameAction.Name);
            if (entity.GetType() == actor.GetType())
            {
                UpdateStats(actor);
            }

            if (entity.GetType() == actee.GetType())
            {
                UpdateStats(actee);
            }
        }
        private void OnNutsChanged(Entity e, int nuts)
        {
            if (e == this.entity.Entity)
            {
                UpdateStats(entity);
            }
        }

        void OnUpgraded(EntityBehaviour entityBehaviour, Upgrade upgrade)
        {
            UpdateStats(entityBehaviour);
        }

        void UpdateStats(EntityBehaviour entityBehaviour)
        {
            if ((entityBehaviour.Entity is Player && isPlayer == false) ||
                (entityBehaviour.Entity is Enemy && isPlayer)) return;
            
            Logger.Log(entityBehaviour.Entity.GetType().ToString() + " Updating Stats");
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
            
            nutsText.text = $"{entityBehaviour.Entity.Nuts}";
        }
        
        void ClearStats()
        {
            statText.text = "";
            resistanceText.text = "";
            nutsText.text = "";
        }
    }
}
