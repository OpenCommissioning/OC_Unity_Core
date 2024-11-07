using System;
using UnityEngine;

namespace OC.Components
{
    public class TransformMover
    {
        public AxisDirection Direction
        {
            get => _axisDirection;
            set
            {
                _axisDirection = value;
                _direction = Math.GetDirection(_axisDirection);
            }
        }

        public AxisType AxisType
        {
            get => _axisType;
            set => _axisType = value;
        }

        public AxisControlMode AxisControlMode
        {
            get => _axisControlMode;
            set => _axisControlMode = value;
        }
        
        public TransformMover(Transform transform)
        {
            _transform = transform;
            _initPosition = _transform.localPosition;
            _initRotation = _transform.localRotation;
        }
        
        public TransformMover(Transform transform, AxisDirection axisDirection, AxisType type, AxisControlMode control)
        {
            _axisDirection = axisDirection;
            _direction = Math.GetDirection(_axisDirection);
            _axisType = type;
            _axisControlMode = control;
            _transform = transform;
            _initPosition = _transform.localPosition;
            _initRotation = _transform.localRotation;
        }

        private AxisDirection _axisDirection = AxisDirection.X;
        private AxisType _axisType = AxisType.Translation;
        private AxisControlMode _axisControlMode = AxisControlMode.Position;
        private Vector3 _direction;
        private float _value;
        private readonly Vector3 _initPosition;
        private readonly Quaternion _initRotation;
        private readonly Transform _transform;

        public void SetConfig(AxisDirection direction, AxisType type, AxisControlMode control)
        {
            _axisDirection = direction;
            _direction = Math.GetDirection(_axisDirection);
            _axisType = type;
            _axisControlMode = control;
        }
        
        public void MoveTo(float value)
        {
            if (Math.FastApproximately(_value, value)) return;
            _value = value;
            Move(_value);
        }

        public void MoveWithSpeed(float value, float deltaTime)
        {
            if (Math.FastApproximately(value, 0f)) return;
            if (Math.FastApproximately(deltaTime, 0f)) return;
            _value += value * deltaTime;
            Move(_value);
        }

        private void Move(float value)
        {
            switch (_axisType)
            {
                case AxisType.Translation:
                    _transform.localPosition = _initPosition + _transform.localRotation * _direction * value;
                    break;
                case AxisType.Rotation:
                    _transform.localRotation = _initRotation * Quaternion.Euler(_direction * value);
                    break;
                case AxisType.Virtual:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_axisType), _axisType, null);
            }
        }
    }
}
