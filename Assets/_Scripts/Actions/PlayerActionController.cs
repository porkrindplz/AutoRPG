using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using __Scripts.Systems;
using _Scripts.Entities;
using _Scripts.Models;
using _Scripts.UI;
using _Scripts.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts.Actions
{
    public class PlayerActionController : MonoBehaviour
    {
        public List<(GameAction action, CountdownTimer timer)?> actionSlots;
        [SerializeField] private EntityBehaviour currentEntity;
        [SerializeField] private EntityBehaviour opposingEntity;

        
        private List<Button> Buttons;
        private List<EventTrigger> EventTriggers;
        private RectTransform[] cooldowns;

        private List<KeyCode> slotButtons;

        private List<string> slotTreeNames;
        
        CharacterAnimationController AnimationController;
        Coroutine _actionCoroutine;

        private void Awake()
        {
            AnimationController = GetComponent<CharacterAnimationController>();
            //Animator = GetComponentInChildren<Animator>();
            GameManager.Instance.OnUpgraded += OnUpgraded;
            GameManager.Instance.OnResetTree += OnResetTree;
            slotTreeNames = new List<string>() { "Sword", "Shield", "Staff", "Slingshot" };
            //GameManager.Instance.OnBeforeGameStateChanged += OnBeforeGameStateChanged;
        }

        private void OnResetTree(UpgradeTree tree)
        {
            for (var i = 0; i < slotTreeNames.Count; i++)
            {
                if (tree.Name == slotTreeNames[i])
                {
                    actionSlots[i] = null;
                    Buttons[i].gameObject.GetComponent<Image>().sprite = null;
                }
            }
        }

        private void OnUpgraded(UpgradeTree tree, Upgrade obj)
        {
            var highestUpgrade = tree.GetHighestLevelAction();
            if (highestUpgrade == null) return;
            
            for (var i = 0; i < slotTreeNames.Count; i++)
            {
                if (tree.Name == slotTreeNames[i])
                {
                    var atk = AttackTypeConverter.StringToAttackType(highestUpgrade.Name);
                    
                    var highestLevelAction = GameManager.Instance.AllActions[atk.Value];
                    if (actionSlots[i]?.action.Name != highestLevelAction.Name)
                    {
                        actionSlots[i] = (highestLevelAction,
                            new CountdownTimer((float)highestLevelAction.TimeToExecute * currentEntity.Entity.GetSpeedMultiplier()));
                        Buttons[i].gameObject.GetComponent<Image>().sprite = actionSlots[i].Value.action.QueueIcon;
                    }
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            actionSlots = new List<(GameAction action, CountdownTimer timer)?>
            {
                null, null, null, null
                //(GameManager.Instance.GetNewAction("attack").GameAction, new CountdownTimer(1)),
                //(GameManager.Instance.GetNewAction("block").GameAction, new CountdownTimer(3)),
            };
            currentEntity = GetComponentInParent<EntityBehaviour>();
            Buttons = GetComponentsInChildren<Button>().ToList();
            cooldowns = new RectTransform[Buttons.Count];
            for (int i = 0; i<Buttons.Count; i++)
            {
                cooldowns[i] = Buttons[i].transform.Find("Cooldown").GetComponent<RectTransform>();
            }
            EventTriggers = Buttons.Select(b => b.GetComponent<EventTrigger>()).ToList();
            Buttons.ForEach(b => b.enabled = false);
            slotButtons = new List<KeyCode> { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };

            for (int i = 0; i < actionSlots.Count; i++)
            {
                //Buttons[i].gameObject.GetComponent<Image>().sprite = actionSlots[i].Value.action.QueueIcon;
                var i1 = i;
                Buttons[i].onClick.AddListener(() =>
                {
                    TryAct(i1, false);
                });
                
            }
        }

        // Update is called once per frame
        void Update()
        {
            var i = 0;
            foreach (var slot in actionSlots)
            {
                slot?.timer.Tick(Time.deltaTime);
                Buttons[i].enabled = slot?.timer.IsFinished == true;
                i++;
            }

            for (i = 0; i < slotButtons.Count; i++)
            {
                TryAct(i, true);
            }
        }

        private void TryAct(int i, bool isKeyDown)
        {
            if ((!Input.GetKeyDown(slotButtons[i]) && isKeyDown) || actionSlots[i]?.timer.IsFinished != true) return;
            if (GameManager.Instance.CurrentGameState != EGameState.Playing) return;
            if (_actionCoroutine != null) return;
            _actionCoroutine = StartCoroutine(ProcessCoroutine(i));

        }

        IEnumerator ProcessCoroutine(int i)
        {
            actionSlots[i]?.timer.Reset();
            actionSlots[i]?.timer.Start();
            var takenAction = actionSlots[i]?.action;
            var processedAction = GameManager.Instance.GetNewAction(takenAction.Name);
            var actee = takenAction.IsSelfTargetting ? currentEntity : opposingEntity;
            float animationTime = AnimationController.AttackAnimation(currentEntity, actee, processedAction);
            AudioSystem.Instance.PlaySound(takenAction.SoundEffects,.5f,true);

            yield return new WaitForSeconds(animationTime);
           
            processedAction?.Interact(currentEntity, actee);
            GameManager.Instance.OnAction?.Invoke(currentEntity, actee, processedAction);  
            yield return Cooldown(takenAction.TimeToExecute * currentEntity.Entity.GetSpeedMultiplier(), cooldowns[i]);
            _actionCoroutine = null;
        }
        
        IEnumerator Cooldown(double cooldown, RectTransform cd)
        {
            float time = 0;
            
            cd.localScale = Vector2.one;
            
        
            while (time < cooldown)
            {
                time += Time.deltaTime;
                cd.localScale = new Vector3(((float)cooldown-time) / (float)cooldown, 1, 1);
                yield return null;
            }
            cd.localScale = Vector2.zero;

        }

        public void GenerateToolTip(int i)
        {
            if (actionSlots[i] == null) return;
            ToolTip.Instance.ShowToolTip(actionSlots[i]?.action.name);
        }
        public void HideToolTip()
        {
            ToolTip.Instance.HideToolTip();
        }
    }
}