using UnityEngine;

namespace _Scripts.Utilities
{
    /// <summary>
    /// A collection of helper methods that can be used throughout the project.
    /// </summary>

    public static class Helpers
    {
        /// <summary>
        /// Destroy all child objects of this transform.
        /// Use it as follows:
        /// <code>
        /// transform.DestroyChildren();
        /// </code>
        /// </summary>
        public static void DestroyChildren(this Transform t)
        {
            foreach (Transform child in t) Object.Destroy(child.gameObject);
        }
    }
}