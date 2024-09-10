using System;
using System.Collections.Generic;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Models;
using UnityEngine;
using UnityEngine.UI;
using Logger = _Scripts.Utilities.Logger;

namespace _Scripts.UI
{
    public class ResistanceUpdater : MonoBehaviour
    {
        Entity _entity;
        [SerializeField] RectTransform resistancePanel;
        List<RectTransform> resistances = new List<RectTransform>();

        Dictionary<string,ModifierChart> ModifierMemory = new Dictionary<string, ModifierChart>();
    
        private void Awake()
        {
        
            foreach (RectTransform resistance in resistancePanel.GetComponentsInChildren<RectTransform>())
            {
                if (resistance.name != "Resistances")
                    resistances.Add(resistance);
            }
        }
        private void OnEnable()
        {
            GameManager.Instance.EnemyManager.OnEnemySpawned += CheckModifierMemory;
            GameManager.Instance.OnAction += UpdateEnemyResistances;
        }
    
        private void OnDisable()
        {
            if(GameManager.Instance!=null)
                GameManager.Instance.OnAction -= UpdateEnemyResistances;
        }

        void CheckModifierMemory(Enemy enemy)
        {
            _entity = GetComponent<EntityBehaviour>().Entity;

            //if enemy name is in dictionary, use modifier memory to populate resistances
            if (ModifierMemory.ContainsKey(enemy.Name))
            {
                ModifierChart memory = ModifierMemory[enemy.Name];
                foreach (RectTransform resistance in resistances)
                {
                    string type = resistance.name;
                    ModifierType modifierType = (ModifierType)Enum.Parse(typeof(ModifierType), type);

                    if(memory.GetType().GetField(type)== null)
                        continue;
                    ModifierType value = GetModifierType(type, memory);
                    Color color = Color.white;
                    color = GetModifierColor(value);
                    UpdateResistanceIndicators(type, color);
                }
            }
        }
    
        private void UpdateEnemyResistances(EntityBehaviour actor, EntityBehaviour actee, IGameAction action)
        {
            int test = 0;
            Logger.Log("Test: " + test);
            _entity = actee.Entity;
            var name = _entity is Enemy enemy ? enemy.Name : ""; 
            Logger.Log("Name is " + name);
        
            if (name!="")
            {
                test++;
                Logger.Log("Test: " + test);
                string type =action.GameAction.Element.ToString();
                if (type == "None")
                {
                    test++;
                    Logger.Log("Test: " + test);
                    type = action.GameAction.AttackGroupType.ToString();
                    Logger.Log("Type is " + type);
                    if (type == "Magic")
                        return;
                }
                //Check memory for enemy resistances
                ModifierChart memory = null;
                ModifierType value = GetModifierType(type, _entity.ReceivedModifiers);
                if (ModifierMemory.ContainsKey(name))
                {
                    memory = ModifierMemory[name];
                    Logger.Log("Memory exists");
                    if (memory.GetType().GetField(type) == null)
                    {
                        value = GetModifierType(type, memory);
                        memory.GetType().GetField(type).SetValue(memory, value);
                        return;
                    }
                }
                else
                {
                    Logger.Log("Create new memory");
                    memory = new ModifierChart();
                    memory.GetType().GetField(type).SetValue(memory, value);
                    ModifierMemory.Add(name, memory);
                }
                Color color = Color.white;
                color = GetModifierColor(value);
                UpdateResistanceIndicators(type, color);
            
            }
        }
        private Color GetModifierColor(ModifierType type)
        {
            switch (type)
            {
                case(ModifierType.Absorb):
                    return Color.green;
                case(ModifierType.Resistant):
                    return Color.yellow;
                case(ModifierType.Weak):
                    return Color.red;
                case(ModifierType.Vulnerable):
                    return Color.magenta;
                case(ModifierType.Neutral):
                    return Color.white;
                case(ModifierType.Null):
                    return Color.grey;
                default:
                    return Color.white;
            }
        }
        private void UpdateResistanceIndicators(string type, Color color)
        {
            foreach (RectTransform resistance in resistances)
            {
                if(resistance.name == type)
                    resistance.GetChild(0).GetComponentInChildren<Image>().color = color;
            }
        }
    
        private ModifierType GetModifierType(string type, ModifierChart chart)
        {
            return (ModifierType)chart.GetType().GetField(type).GetValue(chart);
        }
    }
}
