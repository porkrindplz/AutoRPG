
using System;
using _Scripts.Entities;
using _Scripts.Models;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Actions
{
    public interface IGameAction
    {

        public bool HasStarted { get; set; }
        public event Action OnFinished;
        
        GameAction GameAction { get; set; }

        /// <summary>
        /// Generic interface for two entities to interact with each other
        /// </summary>
        void Interact(EntityBehaviour actor, EntityBehaviour actee);

        void Update(float deltaTime);
    }
}