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
using _Scripts.UI;
using _Scripts.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class AutoAction : MonoBehaviour
{
    
    [SerializeField] private EntityBehaviour currentEntity;
    [SerializeField] private EntityBehaviour opposingEntity;
    [SerializeField] private int maxQueuedActions = 3;
    [SerializeField] private Image shieldImage;
    [SerializeField] private Animator shieldAnimator;

    // 0 is fire, 1 is leaf, 2 is water
    [SerializeField] private List<Sprite> shieldSprites;
    
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
        shieldImage.enabled = false;
        ActionQueue = new Queue<IGameAction>();
        AnimationController = GetComponent<CharacterAnimationController>();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnBeforeGameStateChanged += OnStateChanged;
        EnemyManager.Instance.OnEnemySpawned += OnEnemyChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return; GameManager.Instance.OnBeforeGameStateChanged -= OnStateChanged;
        EnemyManager.Instance.OnEnemySpawned -= OnEnemyChanged;
    }
    void OnStateChanged(EGameState prevState, EGameState state)
    {
        StopAllCoroutines();
    }
    void OnEnemyChanged(Enemy enemy)
    {
        StopAllCoroutines();

    }

    private void Start()
    {
        EnemyManager.Instance.OnEnemySpawned += enemy =>
        {
            ActionQueue.Clear();
            enemyAi = GetComponent<EnemyAI>();
            if (_actionCoroutine != null)
            {
                StopCoroutine(_actionCoroutine);
                _actionCoroutine = null;
            }

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

        // Only queue up attack if tooltips are disabled
        if (ActionQueue.Count == 0 && GameManager.Instance.CurrentGameState == EGameState.Playing && !ToolTip.Instance.IsTutorialActive())
        {
            AddAction();
        }

        if (currentEntity.Entity.ActiveEffects.Count == 0)
        {
            shieldImage.enabled = false;
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
        AddAction(enemyAi.MakeDecision());
    }

    public void AddAction(AttackType attack)
    {
        ActionQueue.Enqueue(GameManager.Instance.GetNewAction(attack));
    }
    
    IEnumerator ProcessAction()
    {
        var takenAction = ActionQueue.Dequeue();
        Debug.Log("Executing Action " + takenAction.GameAction.Name);
        var processedAction = GameManager.Instance.GetNewAction(takenAction.GameAction.Name);
        var actee = takenAction.GameAction.IsSelfTargetting ? currentEntity : opposingEntity;
        float animationTime = AnimationController.AttackAnimation(currentEntity, actee, processedAction);
        StartCoroutine(DelaySoundPlay(takenAction.GameAction.SoundEffects));
        yield return new WaitForSeconds(animationTime);
        
        processedAction?.Interact(currentEntity, actee);

        if (takenAction.GameAction.Name is AttackType.ShieldFire)
        {
            shieldImage.sprite = shieldSprites[0];
            shieldImage.enabled = true;
        }
        if (takenAction.GameAction.Name is AttackType.ShieldLeaf)
        {
            shieldImage.sprite = shieldSprites[1];
            shieldImage.enabled = true;
        }
        if (takenAction.GameAction.Name is AttackType.ShieldWater)
        {
            shieldImage.sprite = shieldSprites[2];
            shieldImage.enabled = true;
        }
        
        GameManager.Instance.OnAction?.Invoke(currentEntity, actee, processedAction);
        AddAction();
        _actionCoroutine = null;
        /*
        */
        //yield return Cooldown(takenAction.TimeToExecute, cooldowns[i]); Need cooldown on enemy attacks
    }
    
    IEnumerator DelaySoundPlay(AudioClip[] clips)
    {
        yield return new WaitForSeconds(.2f);
        AudioSystem.Instance.PlaySound(clips,.5f,true);
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