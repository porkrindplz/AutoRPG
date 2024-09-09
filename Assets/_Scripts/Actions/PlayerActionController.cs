using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Entities;
using _Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Actions
{
    public class PlayerActionController : MonoBehaviour
    {
        public List<(GameAction action, CountdownTimer timer)?> actionSlots;
        [SerializeField] private EntityBehaviour currentEntity;
        [SerializeField] private EntityBehaviour opposingEntity;

        
        private List<Button> Buttons;
        private RectTransform[] cooldowns;

        private List<KeyCode> slotButtons;
        
        
        CharacterAnimationController AnimationController;
        Coroutine _actionCoroutine;
        
        void Awake(){
            AnimationController = GetComponent<CharacterAnimationController>();
        }

        // Start is called before the first frame update
        void Start()
        {
            actionSlots = new List<(GameAction action, CountdownTimer timer)?>
            {
                (GameManager.Instance.GetNewAction("attack").GameAction, new CountdownTimer(1)),
                (GameManager.Instance.GetNewAction("block").GameAction, new CountdownTimer(3)),
            };
            currentEntity = GetComponentInParent<EntityBehaviour>();
            Buttons = GetComponentsInChildren<Button>().ToList();
            cooldowns = new RectTransform[Buttons.Count];
            for (int i = 0; i<Buttons.Count; i++)
            {
                cooldowns[i] = Buttons[i].transform.Find("Cooldown").GetComponent<RectTransform>();
            }
            Buttons.ForEach(b => b.enabled = false);
            slotButtons = new List<KeyCode> { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };

            for (int i = 0; i < actionSlots.Count; i++)
            {
                Buttons[i].gameObject.GetComponent<Image>().sprite = actionSlots[i].Value.action.QueueIcon;
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
            _actionCoroutine = StartCoroutine(ProcessCoroutine(i));

            // actionSlots[i]?.timer.Reset();
            // actionSlots[i]?.timer.Start();
            // var takenAction = actionSlots[i]?.action;
            // var processedAction = GameManager.Instance.GetNewAction(takenAction.Name);
            // var actee = takenAction.IsSelfTargetting ? currentEntity : opposingEntity;
            // processedAction?.Interact(currentEntity, actee);
            // GameManager.Instance.OnAction?.Invoke(currentEntity, actee, processedAction);   
        }

        IEnumerator ProcessCoroutine(int i)
        {
            actionSlots[i]?.timer.Reset();
            actionSlots[i]?.timer.Start();
            var takenAction = actionSlots[i]?.action;
            var processedAction = GameManager.Instance.GetNewAction(takenAction.Name);
            var actee = takenAction.IsSelfTargetting ? currentEntity : opposingEntity;
            float animationTime = AnimationController.AttackAnimation(currentEntity, actee, processedAction);
            yield return new WaitForSeconds(animationTime);
           
            processedAction?.Interact(currentEntity, actee);
            GameManager.Instance.OnAction?.Invoke(currentEntity, actee, processedAction);  
            yield return Cooldown(takenAction.TimeToExecute, cooldowns[i]);

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
    }
}