using System.Collections;
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
        public IPropertyReadOnly<float> Delta => _delta;

        [SerializeField]
        private Property<float> _speed = new (100);
        [SerializeField]
        private Property<float> _delta = new (0);

        private IEnumerator _isActiveCoroutine;

        private void Awake()
        {
            _isActiveCoroutine = IsActiveCoroutine();
        }

        protected override void GetLinkData()
        {
            _target.Value = _connectorData.ControlData;
        }
        
        protected override void Operation(float deltaTime)
        {
            if (_speed.Value > Utils.TOLERANCE_HALF)
            {
                _delta.Value = _target.Value - _value.Value;
                _value.Value = Mathf.MoveTowards(_value.Value, _target.Value, _speed.Value * deltaTime);
                _isActive.Value = Mathf.Abs(_target.Value - _value.Value) > Utils.TOLERANCE_HALF;
            }
            else
            { 
                _delta.Value = _target.Value - _value.Value;
                _value.Value = _target.Value;
                StopCoroutine(_isActiveCoroutine);
                StartCoroutine(_isActiveCoroutine);
            }
        }

        private IEnumerator IsActiveCoroutine()
        {
            _isActive.Value = true;
            yield return new WaitForSeconds(.1f);
            _isActive.Value = false;
        }

        protected override void SetLinkData()
        {
            _connectorData.StatusData = _value.Value;
            _connectorData.Status.SetBit(6, _isActive);
        }
    }
}