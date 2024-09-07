using System;
using System.Linq;
using _Scripts.Entities;
using _Scripts.Models;
using TMPro;
using UnityEngine;
using ElementsType = _Scripts.Models.ElementsType;

namespace _Scripts.UI
{
    public class UIStatsController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statText;
        [SerializeField] private TextMeshProUGUI resistanceText;
        [SerializeField] private bool isPlayer;
    
        // Start is called before the first frame update
        void Start()
        {
            GameManager.Instance.OnUpgraded += OnUpgraded;
        }

        private void OnDestroy()
        {
            GameManager.Instance.OnUpgraded -= OnUpgraded;
        }

        void OnUpgraded(EntityBehaviour entityBehaviour, Upgrade upgrade)
        {
            if (entityBehaviour.Entity.IsPlayer != isPlayer) return;
            
            statText.text = $"Health: {(int)entityBehaviour.Entity.CurrentHealth}/{(int)entityBehaviour.Entity.MaxHealth}\n" +
                            $"Attack: {(int)entityBehaviour.Entity.GetTotalAttack()}" +
                            $"Defense: {(int)entityBehaviour.Entity.GetTotalDefense()}" +
                            $"Speed: {entityBehaviour.Entity.Speed}";
            var resText = "Resistances\n";
            foreach (var element in Enum.GetValues(typeof(ElementsType)).Cast<ElementsType>())
            {
                resText += $"{element.ToString()}: {entityBehaviour.Entity.Resistances}\n";
            }

            statText.text = resText;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
