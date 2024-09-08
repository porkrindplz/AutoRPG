using _Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Actions
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameAction", order = 1)]
    public class GameAction : ScriptableObject
    {
        /// <summary>
        /// Name to refer to the GameAction by. MUST also have a corresponding entry in GameManager.AllActions
        /// dictionary in order to actually perform the logic of this action
        /// </summary>
        public string Name;
        
        /// <summary>
        /// Time in seconds it takes to execute the action once it is first in the action queue
        /// </summary>
        public double TimeToExecute;
        
        /// <summary>
        /// Graphic of the action as it will appear in the action queue for the acting character
        /// </summary>
        public Sprite QueueIcon;
        
        /// <summary>
        /// Elemental affinity of the attack, can be set to None for non-elemental
        /// </summary>
        public ElementsType Element;
        
        /// <summary>
        /// If set to true, the actee of this action will be itself when executed
        /// </summary>
        public bool IsSelfTargetting;
    }
}