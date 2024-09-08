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

        private List<KeyCode> slotButtons;
    
        // Start is called before the first frame update
        void Start()
        {
            actionSlots = new List<(GameAction action, CountdownTimer timer)?>
            {
                (GameManager.Instance.AllActions["attack"].GameAction, new CountdownTimer(1)),
                (GameManager.Instance.AllActions["block"].GameAction, new CountdownTimer(3)),
            };
            currentEntity = GetComponentInParent<EntityBehaviour>();
            Buttons = GetComponentsInChildren<Button>().ToList();
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
            
            actionSlots[i]?.timer.Reset();
            actionSlots[i]?.timer.Start();
            var takenAction = actionSlots[i]?.action;
            var processedAction = GameManager.Instance.AllActions[takenAction.Name];
            var actee = takenAction.IsSelfTargetting ? currentEntity : opposingEntity;
            processedAction?.Interact(currentEntity.Entity, actee.Entity);
            GameManager.Instance.OnAction?.Invoke(currentEntity, actee, processedAction);   
        }
    }
}