using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Utilities;
using UnityEngine;
using Random = System.Random;

public class AutoAction : MonoBehaviour
{

    [SerializeField] public List<GameAction> possibleActions;
    [SerializeField] public List<double> weights;
    [SerializeField] private EntityBehaviour currentEntity;
    [SerializeField] private EntityBehaviour opposingEntity;
    [SerializeField] private int maxQueuedActions = 3;
    
    public Queue<GameAction> ActionQueue;
    
    private double _timer;
    private WeightedRouletteWheel _weighter;
    
    // Start is called before the first frame update
    void Start()
    {
        _weighter = new WeightedRouletteWheel();
        ActionQueue = new Queue<GameAction>();
    }

    public void PopulateQueue()
    {
        if (possibleActions.Count == 0) return;
        for (int i = 0; i < maxQueuedActions; i++)
        {
            ActionQueue.Enqueue(_weighter.SelectItem(possibleActions, weights));
        }
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if (ActionQueue is { Count: > 0 } && _timer >= ActionQueue.Peek().TimeToExecute)
        {
            _timer = 0;
            var takenAction = ActionQueue.Dequeue();
            Debug.Log("Executing Action " + takenAction.Name);
            var processedAction = GameManager.Instance.AllActions[takenAction.Name];
            var actee = takenAction.IsSelfTargetting ? currentEntity : opposingEntity;
            processedAction?.Interact(currentEntity.Entity, actee.Entity);
            GameManager.Instance.OnAction?.Invoke(currentEntity, actee, takenAction);
            ActionQueue.Enqueue(_weighter.SelectItem(possibleActions, weights));
        }
    }
}