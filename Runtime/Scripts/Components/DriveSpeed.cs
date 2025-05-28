using UnityEngine;
using OC.Communication;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Actor/Drive Speed")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class DriveSpeed : Drive
    {
        public IProperty<float> Acceleration => _acceleration;

        [SerializeField]
        private Property<float> _acceleration = new (100);

        protected override void GetLinkData()
        {
            _target.Value = _connectorData.ControlData;
        }
        
        protected override void Operation(float deltaTime)
        {
            _value.Value = Mathf.MoveTowards(_value.Value, _target, _acceleration * deltaTime);
            _stateObserver.SetDelta(_value.Value);
        }

        protected override void SetLinkData()
        {
            _connectorData.StatusData = _value.Value;
            _connectorData.Status.SetBit(6, _stateObserver.IsActive.Value);
        }
    }
}
