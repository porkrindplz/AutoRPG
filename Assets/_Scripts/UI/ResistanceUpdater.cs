using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Actions;
using _Scripts.Entities;
using _Scripts.Models;
using UnityEngine;

public class ResistanceUpdater : MonoBehaviour
{
    Entity _entity;
    List<RectTransform> resistances = new List<RectTransform>();

    private void OnEnable()
    {
        GameManager.Instance.OnAction += UpdateEnemyResistances;
    }
    
    private void OnDisable()
    {
        if(GameManager.Instance!=null)
            GameManager.Instance.OnAction -= UpdateEnemyResistances;
    }
    
    private void UpdateEnemyResistances(EntityBehaviour actor, EntityBehaviour actee, IGameAction action)
    {
        if (actee.Entity.GetType() == typeof(Enemy))
        {
            // action.GameAction.Element
            // _entity.ReceivedModifiers.;
        }
    }
}
