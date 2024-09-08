using System;
using _Scripts.Models;
using _Scripts.Utilities;

namespace _Scripts.Actions.Effects
{
    public enum ActiveEffectType
    {
        Block,
        Burn,
        Slow,
        AtkUp
    }
    
    public class ActiveEffect
    {
        public bool IsEffectFinished => _timer.IsFinished;
        public ActiveEffectType ActiveEffectType;
        private CountdownTimer _timer;
        

        public ActiveEffect(ActiveEffectType activeType, float activeTime)
        {
            ActiveEffectType = activeType;
            _timer = new CountdownTimer(activeTime);
            _timer.Start();
        }

        public void Update(float deltaTime)
        {
            _timer.Tick(deltaTime);
        }
    }
}