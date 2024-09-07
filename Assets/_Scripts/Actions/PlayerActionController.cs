using System.Collections.Generic;
using System.Linq;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

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
        currentEntity = GetComponentInParent<EntityBehaviour>();
        Buttons = GetComponentsInChildren<Button>().ToList();
        Buttons.ForEach(b => b.enabled = false);
        slotButtons = new List<KeyCode> { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };
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
            if (!Input.GetKeyDown(slotButtons[i]) || actionSlots[i]?.timer.IsFinished != true) continue;
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