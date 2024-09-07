using _Scripts.Utilities;
using UnityEngine;
using Logger = _Scripts.Utilities.Logger;

namespace _Scripts.UI
{
    public class SkillTree : Singleton<SkillTree>
    {
        public void UpgradeNode(string id)
        {
            Logger.Log($"Upgrading node {id}");
        }
    }
}
