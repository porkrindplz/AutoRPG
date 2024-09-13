using System;
using _Scripts.Models;
using _Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.UI
{
    public class ToolTip : Singleton<ToolTip>
    {
        [Header("Tool Tip")]
        [SerializeField] RectTransform toolTipRect;
        [SerializeField] TextMeshProUGUI toolTipText;

        [Header("Tutorials")]
        [SerializeField]private StorySO[] Tutorials;

        private int tutorialIndex = 0;

        private StorySO ActiveTutorial;

        [SerializeField] private RectTransform tutorialRect;
        [SerializeField] private TextMeshProUGUI tutorialText;
        protected override void Awake()
        {
            base.Awake();
            HideToolTip();
        }

        public bool IsTutorialActive() => tutorialRect.gameObject.activeSelf;

        private void OnEnable()
        {
            GameManager.Instance.OnBeforeGameStateChanged += OnGameStateChanged;
            GameManager.Instance.OnUpgraded += OnUpgrade;

        }
        
        void OnGameStateChanged(EGameState prevState, EGameState state)
        {
            if (state == EGameState.Playing)
            {
                if(GameManager.Instance.PlayStats.TotalVictories == 0 && Tutorials.Length > tutorialIndex)
                {
                    ShowTutorial(0);
                }
                if(GameManager.Instance.PlayStats.TotalVictories == 1 && Tutorials.Length > tutorialIndex)
                {
                    ShowTutorial(2);
                }

                if (GameManager.Instance.PlayStats.TotalVictories == 2 && Tutorials.Length > tutorialIndex)
                {
                    ShowTutorial(3);
                }
            }
        }

        void OnUpgrade(UpgradeTree Tree, Upgrade upgrade)
        {
            if (ActiveTutorial == Tutorials[0] && upgrade.Id== "sword")
            {
                HideTutorial();
                tutorialIndex++;
                ShowTutorial(tutorialIndex);
                return;
            }
            if (ActiveTutorial == Tutorials[1] && upgrade.Id== "block")
            {
                HideTutorial();
                tutorialIndex++;

            }
            if(ActiveTutorial == Tutorials[2] && upgrade.Id == "fireball")
            {
                HideTutorial();
                tutorialIndex++;

            }
            if(ActiveTutorial == Tutorials[3] && upgrade.Id == "aoe" || upgrade.Id == "cross_slash")
            {
                HideTutorial();
                tutorialIndex++;

            }
        }
        protected void Update()
        {
            if(toolTipRect.gameObject.activeSelf)
            {
                toolTipRect.position = Input.mousePosition;
            }

        }
        public void ShowTutorial(int tipIndex)
        {
            ActiveTutorial = Tutorials[tipIndex];
            tutorialRect.gameObject.SetActive(true);

            tutorialText.text = tutorialText.text = Tutorials[tipIndex].Pages[0].text;
        }
        
        public void HideTutorial()
        {
            tutorialText.text = "";
            ActiveTutorial = null;
            tutorialRect.gameObject.SetActive(false);
        }
    
        public void ShowToolTip(string message)
        {
            if (message == null)
            {
                HideToolTip();
                return;
            }

            toolTipRect.gameObject.SetActive(true);
            toolTipText.text = message;
            // CURSED CODE
            toolTipRect.position = Input.mousePosition;
            if (toolTipRect.position.x > 640)
            {
                toolTipRect.transform.localScale = new Vector3(-1, 1, 1);
                toolTipRect.transform.GetChild(0).transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                toolTipRect.transform.localScale = new Vector3(1, 1, 1);
                toolTipRect.transform.GetChild(0).transform.localScale = new Vector3(1, 1, 1);
            }
        }
    
        public void HideToolTip()
        {
            toolTipText.text = "";
            toolTipRect.gameObject.SetActive(false);
        }
    }
}
