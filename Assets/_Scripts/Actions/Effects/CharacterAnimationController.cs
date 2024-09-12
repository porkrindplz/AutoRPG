using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Models;
using UnityEngine;
using UnityEngine.UI;
using Logger = _Scripts.Utilities.Logger;

public class CharacterAnimationController : MonoBehaviour
{

    public RectTransform EntityImageRect;
    private Animator animator;
    private EntityBehaviour entity;
    
    Color takeDamageColor = Color.red;
    Color defaultColor = Color.white;

    private void Awake()    
    {
        animator = GetComponentInChildren<Animator>();
        entity = GetComponent<EntityBehaviour>();
    }

    private void OnEnable()
    {
        
        GameManager.Instance.OnAction += ActeeHitAnimation;
    }

    private void OnDisable()
    {
        if(GameManager.Instance!=null)
            GameManager.Instance.OnAction -= ActeeHitAnimation;
    }

    public float AttackAnimation(EntityBehaviour actor, EntityBehaviour actee, IGameAction action)
    {
        if (actor != entity) return 0;
        animator.SetTrigger("OnAttack");

        return animator.GetCurrentAnimatorStateInfo(0).length;
    }
    public void DeathAnimation(Entity actor)
    {
        if (actor != entity.Entity) return;
        animator.SetTrigger("OnDeath");
    }

    public void ActeeHitAnimation(EntityBehaviour actor, EntityBehaviour actee, IGameAction action)
    {
        if (actee != entity) return;
        
        Logger.Log($"Entity: {entity.Entity} is taking damage"); ;
        
        string type = action.GameAction.Name.ToString();
        Logger.Log("Type: " + type);
        if(action is AttackAction attackAction)
        {
            if (type.Contains("Shield"))
            {
                Logger.Log("Blocking?");
                animator.SetTrigger("OnBlock");
            }
            else
            {
                Logger.Log("Taking Damage");
                animator.SetTrigger("OnHit");
            }
        }
        else if(action is BlockAction blockAction)
            animator.SetTrigger("OnBlock");

        
    }

}
