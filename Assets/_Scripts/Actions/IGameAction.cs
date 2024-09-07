
using _Scripts.Models;

namespace _Scripts.Actions
{
    public interface IGameAction
    {
        /// <summary>
        /// Generic interface for two entities to interact with each other
        /// </summary>
        void Interact(Entity actor, Entity actee);
    }
}