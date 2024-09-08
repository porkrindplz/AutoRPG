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

    [SerializeField] public List<string> possibleActions;
    [SerializeField] public List<double> weights;
    [SerializeField] private EntityBehaviour currentEntity;
    [SerializeField] private EntityBehaviour opposingEntity;
    [SerializeField] private int maxQueuedActions = 3;
    
    public Queue<IGameAction> ActionQueue;
    
    private double _timer;
    private WeightedRouletteWheel _weighter;
    
    private double _nutTimer;
    private int _nutInterval = 5;
    
    // Start is called before the first frame update
    void Awake()
    {
        _weighter = new WeightedRouletteWheel();
        ActionQueue = new Queue<IGameAction>();
    }

    public void PopulateQueue()
    {
        if (possibleActions.Count == 0) return;
        for (int i = 0; i < maxQueuedActions; i++)
        {
            var newAction = GameManager.Instance.GetNewAction(_weighter.SelectItem(possibleActions, weights));
            ActionQueue.Enqueue(newAction);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentGameState != EGameState.Playing)
        {
            return;
        }
        
        //if (ActionQueue?.Peek().)
        
        _timer += Time.deltaTime;
        if (ActionQueue is { Count: > 0 } && _timer >= ActionQueue.Peek().GameAction.TimeToExecute)
        {
            _timer = 0;
            var takenAction = ActionQueue.Dequeue();
            Debug.Log("Executing Action " + takenAction.GameAction.Name);
            var processedAction = GameManager.Instance.GetNewAction(takenAction.GameAction.Name);
            var actee = takenAction.GameAction.IsSelfTargetting ? currentEntity : opposingEntity;
            processedAction?.Interact(currentEntity, actee);
            GameManager.Instance.OnAction?.Invoke(currentEntity, actee, processedAction);
            var newAction = GameManager.Instance.GetNewAction(_weighter.SelectItem(possibleActions, weights));
            ActionQueue.Enqueue(newAction);
        }
        
        if (currentEntity.Entity.Nuts > 1)
        {
            _nutTimer += Time.deltaTime;
            if (_nutTimer >= _nutInterval)
            {
                _nutTimer = 0;
                currentEntity.Entity.Nuts--;
            }
        }
    }
}