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
    
    Coroutine _actionCoroutine;
    CharacterAnimationController AnimationController;
    
    // Start is called before the first frame update
    void Awake()
    {
        _weighter = new WeightedRouletteWheel();
        ActionQueue = new Queue<IGameAction>();
        AnimationController = GetComponent<CharacterAnimationController>();
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
        if (_actionCoroutine==null&&ActionQueue is { Count: > 0 } && _timer >= ActionQueue.Peek().GameAction.TimeToExecute)
        {
            _timer = 0;
            _actionCoroutine = StartCoroutine(ProcessAction());
            // var takenAction = ActionQueue.Dequeue();
            // Debug.Log("Executing Action " + takenAction.GameAction.Name);
            // var processedAction = GameManager.Instance.GetNewAction(takenAction.GameAction.Name);
            // var actee = takenAction.GameAction.IsSelfTargetting ? currentEntity : opposingEntity;
            // processedAction?.Interact(currentEntity, actee);
            // GameManager.Instance.OnAction?.Invoke(currentEntity, actee, processedAction);
            // var newAction = GameManager.Instance.GetNewAction(_weighter.SelectItem(possibleActions, weights));
            // ActionQueue.Enqueue(newAction);
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

        IEnumerator ProcessAction()
        {
            var takenAction = ActionQueue.Dequeue();
            Debug.Log("Executing Action " + takenAction.GameAction.Name);
            var processedAction = GameManager.Instance.GetNewAction(takenAction.GameAction.Name);
            var actee = takenAction.GameAction.IsSelfTargetting ? currentEntity : opposingEntity;
            float animationTime = AnimationController.AttackAnimation(currentEntity, actee, processedAction);
            yield return new WaitForSeconds(animationTime);
            
            processedAction?.Interact(currentEntity, actee);
            GameManager.Instance.OnAction?.Invoke(currentEntity, actee, processedAction);
            var newAction = GameManager.Instance.GetNewAction(_weighter.SelectItem(possibleActions, weights));
            ActionQueue.Enqueue(newAction);
            _actionCoroutine = null;
            //yield return Cooldown(takenAction.TimeToExecute, cooldowns[i]); Need cooldown on enemy attacks
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