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
            GameManager.Instance.OnUpgraded+= UpdatePlayerResistances;
        }
    
        private void OnDisable()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnAction -= UpdateEnemyResistances;
            GameManager.Instance.OnUpgraded -= UpdatePlayerResistances;
            GameManager.Instance.EnemyManager.OnEnemySpawned -= CheckModifierMemory;
        }

        void CheckModifierMemory(Enemy enemy)
        {
            if( enemy == null)
                return;
            _entity = GetComponent<EntityBehaviour>().Entity;

            //if enemy name is in dictionary, use modifier memory to populate resistances
            if (ModifierMemory.ContainsKey(enemy.Name))
            {
                ModifierChart memory = ModifierMemory[enemy.Name];
                foreach (RectTransform resistance in resistances)
                {
                    string type = resistance.name;

                    if((ModifierType)memory.GetType().GetField(type).GetValue(memory) == ModifierType.Empty)
                        continue;
                    ModifierType value = GetModifierType(type, memory);
                    Color color = Color.white;
                    color = GetModifierColor(value);
                    UpdateResistanceIndicators(type, color);
                }
            }
            else
            {

                ModifierMemory.Add(enemy.Name, GenerateCleanChart());
            }
        }
        
        private ModifierChart GenerateCleanChart()
        {
            ModifierChart memory = new ModifierChart();
            memory.Melee = ModifierType.Empty;
            memory.Ranged = ModifierType.Empty;
            memory.Aoe = ModifierType.Empty;
            memory.Fire = ModifierType.Empty;
            memory.Water = ModifierType.Empty;
            memory.Leaf = ModifierType.Empty;
            memory.Lightning = ModifierType.Empty;
            memory.Shadow = ModifierType.Empty;
            return memory;
        }
    
        private void UpdateEnemyResistances(EntityBehaviour actor, EntityBehaviour actee, IGameAction action)
        {
            var name = actee.Entity is Enemy enemy ? enemy.Name : ""; 


            if(name == "")
                return;
            _entity = actee.Entity;
            //Logger.Log("Name is " + name);

            string[] types = new[] {"None","None"};
            int typeIndex = 0;
            types[typeIndex] =action.GameAction.Element.ToString();
            typeIndex++;
            types[typeIndex] = action.GameAction.AttackGroupType.ToString();
            //Logger.Log("Type is " + types[typeIndex]);
            if (types[typeIndex] == "Magic")
            {
                types[typeIndex] = "None";
            }


            
            //Check memory for enemy resistances
            ModifierChart memory = null;
            foreach (string type in types)
            {
                if("None" == type)
                    continue;
                //Logger.Log("Type is " + type);
                ModifierType value = GetModifierType(type, _entity.ReceivedModifiers);
                //Logger.Log("Received modifier is " + value);
                if (ModifierMemory.ContainsKey(name))
                {
                    memory = ModifierMemory[name];
                    //Logger.Log("Memory exists");
                    if ((ModifierType)memory.GetType().GetField(type).GetValue(memory)==ModifierType.Empty)
                    {
                    
                        memory.GetType().GetField(type).SetValue(memory, value);

                    }
                }
                else
                {
                    //Logger.Log("Create new memory");
                    memory = GenerateCleanChart();
                    memory.GetType().GetField(type).SetValue(memory, value);
                    ModifierMemory.Add(name, memory);
                }
                Color color = Color.white;
                color = GetModifierColor(value);
                UpdateResistanceIndicators(type, color);

            }
            // ModifierType value = GetModifierType(type, _entity.ReceivedModifiers);
            // Logger.Log("Received modifier is " + value);
            // if (ModifierMemory.ContainsKey(name))
            // {
            //     memory = ModifierMemory[name];
            //     Logger.Log("Memory exists");
            //     if ((ModifierType)memory.GetType().GetField(type).GetValue(ModifierMemory[name])==ModifierType.Empty)
            //     {
            //         
            //         memory.GetType().GetField(type).SetValue(memory, value);
            //
            //     }
            // }
            // else
            // {
            //     Logger.Log("Create new memory");
            //     memory = GenerateCleanChart();
            //     memory.GetType().GetField(type).SetValue(memory, value);
            //     ModifierMemory.Add(name, memory);
            // }
            // Color color = Color.white;
            // color = GetModifierColor(value);
            // UpdateResistanceIndicators(type, color);

        }
        
        private void UpdatePlayerResistances(UpgradeTree tree, Upgrade upgrade)
        {
            return;
            //_entity = GetComponent<EntityBehaviour>().Entity;
            
            if(upgrade.UpgradeEffects == null)
                return;
            

            if (_entity is Player player)
            {
                //Update once resistances are in for player
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
                case(ModifierType.Empty):
                    return Color.black;
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
        
        public void GenerateToolTip(string type)
        {
            string message = "";
            _entity = GetComponent<EntityBehaviour>().Entity;
            var name = _entity is Enemy enemy ? enemy.Name : ""; 
            
            //Logger.Log("Entity is "  +_entity);
            //Logger.Log("Name is " + name);
            //Logger.Log("Type is " + type);
            
            if (_entity == null || name == "")
                return;
            if(!ModifierMemory.ContainsKey(name) || (ModifierType)ModifierMemory[name].GetType().GetField(type).GetValue(ModifierMemory[name]) == ModifierType.Empty)
                message = $"{type}: ???";
            else
            {
                //Logger.Log("Memory contains: " + ModifierMemory[name].GetType().GetField(type));
                message = $"{type}: {GetModifierType(type, _entity.ReceivedModifiers)}";
            }

            ToolTip.Instance.ShowToolTip(message);
        }

        public void HideToolTip()
        {
            ToolTip.Instance.HideToolTip();
        }
    }
}
