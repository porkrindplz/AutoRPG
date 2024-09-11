using System;
using _Scripts.Utilities;
using UnityEngine;

namespace _Scripts.Entities
{
    public interface IStageChanger
    {
        public event Action ChangeStage;
    }
    
    public class TimerStageChanger : MonoBehaviour, IStageChanger
    {
        public CountdownTimer TimerToStageChange;

        public event Action ChangeStage;

        public void Start()
        {
            TimerToStageChange.Start();
        }

        private void Update()
        {
            TimerToStageChange.Tick(Time.deltaTime);
            if (TimerToStageChange.IsFinished)
            {
                ChangeStage?.Invoke();
                Destroy(this);
            }
        }
    }
}