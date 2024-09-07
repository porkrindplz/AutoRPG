using _Scripts.Models;
using UnityEngine;

namespace _Scripts.Actions
{
    public class BlockGA : IGameAction
    {
        public GameAction GameAction { get; set; }
        public void Interact(Entity actor, Entity actee)
        {
            Debug.Log("Blocked");
        }
    }
}