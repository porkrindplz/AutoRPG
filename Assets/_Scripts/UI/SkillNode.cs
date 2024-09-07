using System;
using _Scripts.Models;
using UnityEngine;

namespace _Scripts.UI
{
    public class SkillNode : MonoBehaviour
    {
        private string parentName;

        private void Awake()
        {
            parentName = transform.parent.name;
        }

        public void OnClick(string id)
        {
            switch (parentName)
            {
                case "WarriorTree":
                    GameManager.Instance.AllTrees.Warrior.TryUpgrade(id);
                    break;
            }
        }
    }
}
