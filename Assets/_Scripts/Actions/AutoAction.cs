using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Actions;
using _Scripts.Entities;
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

public class WeightedRouletteWheel
{
    private Random random = new();

    // Initialize the random number generator

    public T SelectItem<T>(List<T> items, List<double> weights)
    {
        if (items == null || weights == null || items.Count != weights.Count || items.Count == 0)
        {
            throw new ArgumentException("Items and weights must be non-null and of the same length.");
        }

        // Calculate the total weight sum
        double totalWeight = weights.Sum();

        // Generate a random number between 0 and totalWeight
        double randomValue = random.NextDouble() * totalWeight;

        // Iterate through the items and accumulate their weights
        double cumulativeWeight = 0.0;
        for (int i = 0; i < items.Count; i++)
        {
            cumulativeWeight += weights[i];

            // If the random value falls within the cumulative weight range, select the item
            if (randomValue <= cumulativeWeight)
            {
                return items[i];
            }
        }

        // In case of rounding errors, return the last item
        return items[items.Count - 1];
    }
}