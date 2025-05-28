using OC.Communication;
using UnityEngine;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Actor/Drive Position")]
    [SelectionBase]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    public class DrivePosition : Drive
    {
        public IProperty<float> Speed => _speed;

        [SerializeField]
        private Property<float> _speed = new (100);

        protected override void GetLinkData()
        {
            _target.Value = _connectorData.ControlData;
        }
        
        protected override void Operation(float deltaTime)
        {
            if (_speed.Value > Utils.TOLERANCE_HALF)
            {
                _value.Value = Mathf.MoveTowards(_value.Value, _target.Value, _speed.Value * deltaTime);
            }
            else
            { 
                _value.Value = _target.Value;
            }
            
            _stateObserver.Update(_value.Value, deltaTime);
        }

        protected override void SetLinkData()
        {
            _connectorData.StatusData = _value.Value;
            _connectorData.Status.SetBit(6, _stateObserver.IsActive.Value);
        }
    }
}