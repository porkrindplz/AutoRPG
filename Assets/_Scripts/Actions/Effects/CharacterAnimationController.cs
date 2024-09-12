using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAnimationController : MonoBehaviour
{

    public Image EntityImage;
    private Animator animator;
    private EntityBehaviour entity;

    private void Awake()    
    {
        animator = GetComponentInChildren<Animator>();
        entity = GetComponent<EntityBehaviour>();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnAction += ReceiveAnimation;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnAction -= ReceiveAnimation;
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

    private void ReceiveAnimation(EntityBehaviour actor,EntityBehaviour actee,IGameAction action)
    {
        if (actee != entity) return;

        
        if (action is AttackAction)
        {
            StartCoroutine(TakeDamageEffect());
        }
    }
    IEnumerator TakeDamageEffect()
    {
        EntityImage.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        EntityImage.color = Color.white;
    }
}
