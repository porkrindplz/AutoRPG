using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Models;
using _Scripts.Utilities;
using UnityEngine;
using Random = System.Random;

public class AutoAction : MonoBehaviour
{
    
    [SerializeField] private EntityBehaviour currentEntity;
    [SerializeField] private EntityBehaviour opposingEntity;
    [SerializeField] private int maxQueuedActions = 3;
    
    public Queue<IGameAction> ActionQueue;

    private EnemyAI enemyAi;
    
    private double _timer;
    
    private double _nutTimer;
    private int _nutInterval = 5;
    
    Coroutine _actionCoroutine;
    CharacterAnimationController AnimationController;
    
    // Start is called before the first frame update
    void Awake()
    {
        enemyAi = GetComponent<EnemyAI>();
        
        ActionQueue = new Queue<IGameAction>();
        AnimationController = GetComponent<CharacterAnimationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentGameState != EGameState.Playing)
        {
            return;
        }

        if (ActionQueue.Count == 0)
        {
            AddAction();
        }
        
        //if (ActionQueue?.Peek().)
        
        _timer += Time.deltaTime;
        if (_actionCoroutine==null&&ActionQueue is { Count: > 0 } && _timer >= ActionQueue.Peek().GameAction.TimeToExecute * currentEntity.Entity.GetSpeedMultiplier())
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
    }

    private void AddAction()
    {
        ActionQueue.Enqueue( GameManager.Instance.GetNewAction(enemyAi.MakeDecision()));
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

        AddAction();
        _actionCoroutine = null;
        /*
        */
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