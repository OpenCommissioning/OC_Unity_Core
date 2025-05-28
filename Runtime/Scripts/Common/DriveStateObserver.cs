using System;
using OC.Components;
using UnityEngine;

namespace OC
{
    [Serializable]
    public class DriveStateObserver
    {
        public IPropertyReadOnly<Drive.DriveState> State => _state;
        public IPropertyReadOnly<bool> IsActive => _isActive;

        public float Delay
        {
            get => _delay;
            set => _delay = value;
        }
        
        [SerializeField]
        private Property<Drive.DriveState> _state = new(Drive.DriveState.Idle); 
        [SerializeField]
        private Property<bool> _isActive = new(false);
        [SerializeField]
        private float _delay = 0.2f;
        
        private float _delta;
        private float _lastValue;
        private float _timer;

        public void Update(float value, float deltaTime)
        {
            _delta = value - _lastValue;

            if (Mathf.Abs(_delta) < Utils.TOLERANCE)
            {
                if (_timer > 0)
                {
                    _timer -= deltaTime;
                }
                else
                {
                    _timer = 0;
                    SetDelta(0);
                }
            }
            else
            {
                _lastValue = value;
                _timer = _delay;
                SetDelta(_delta);
            }
        }

        public void SetDelta(float delta)
        {
            _state.Value = delta switch
            {
                > 0 => Drive.DriveState.IsRunningPositive,
                < 0 => Drive.DriveState.IsRunningNegative,
                _ => Drive.DriveState.Idle
            };
            
            _isActive.Value = _state.Value != Drive.DriveState.Idle;
        }
    }
}