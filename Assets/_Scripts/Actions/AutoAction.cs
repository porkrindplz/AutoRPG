using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using __Scripts.Systems;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Entities.EnemyAIs;
using _Scripts.Managers;
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
    private int _nutUpdateInterval = 5;
    private double _nutsLostPerUpdateInterval;
    
    Coroutine _actionCoroutine;
    CharacterAnimationController AnimationController;
    
    // Start is called before the first frame update
    void Awake()
    {
        enemyAi = GetComponent<EnemyAI>();
        
        ActionQueue = new Queue<IGameAction>();
        AnimationController = GetComponent<CharacterAnimationController>();
    }

    private void Start()
    {
        EnemyManager.Instance.OnEnemySpawned += enemy =>
        {
            ActionQueue.Clear();
            _timer = 0;
            var currGroup = EnemyManager.Instance.GetCurrentGroup();
            _nutsLostPerUpdateInterval = 0.5*currGroup.MinNutsWon / (currGroup.TimeForNutLoss) *  (float)_nutUpdateInterval;
        };
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
        
        _nutTimer += Time.deltaTime;
        if (_nutTimer >= _nutUpdateInterval)
        {
            _nutTimer = 0;
            var enemyGroup = EnemyManager.Instance.GetCurrentGroup();
            enemyGroup.ActualNutsWon = 
                Math.Max(enemyGroup.MinNutsWon, enemyGroup.ActualNutsWon - (int)_nutsLostPerUpdateInterval);
            GameManager.Instance.OnNutsChanged?.Invoke(currentEntity.Entity, (int)enemyGroup.ActualNutsWon);
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
        AudioSystem.Instance.PlaySound(takenAction.GameAction.SoundEffects,.5f,true);
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