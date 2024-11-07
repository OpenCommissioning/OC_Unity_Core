using System;
using UnityEngine;

namespace OC.Components
{
    public class MeasurementEncoder : MonoBehaviour, IMeasurement<float>
    {
        public IPropertyReadOnly<float> Value => _value;

        [Header("State")]
        [SerializeField]
        private Property<float> _value = new ();

        [Header("Settings")]
        [SerializeField]
        private Drive _drive;
        [SerializeField]
        private DriveType _driveType;
        [SerializeField]
        private float _factor = 1;
        [SerializeField]
        private float _modulo = -1;

        private bool _isValid;

        private void Start()
        {
            _isValid = _drive != null;
        }

        private void FixedUpdate()
        {
            if (!_isValid) return;

            switch (_driveType)
            {
                case DriveType.Position:
                    _value.Value = _drive.Value.Value * _factor % _modulo;
                    break;
                case DriveType.Speed:
                    var position = _value.Value + _drive.Value.Value * _factor * Time.fixedDeltaTime;
                    _value.Value = position % _modulo;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
        private enum DriveType
        {
            Position,
            Speed
        }
    }
}
