using System;
using UnityEngine;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Actor/Axis")]
    [SelectionBase]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(1000)]
    public class Axis : Actor, IInteractable, ICustomInspector
    {
        public Actor Actor
        {
            set => _actor = value;
            get => _actor;
        }

        public float Factor 
        {
            set => _factor = value;
            get => _factor;
        }
        
        public float Offset 
        {
            set => _offset = value;
            get => _offset;
        }
        
        public AxisType Type
        {
            set
            {
                _type = value;
                if (_transformMover != null) _transformMover.AxisType = _type;
            }
            get => _type;
        }

        public AxisDirection Direction
        {
            set
            {
                _direction = value;
                if (_transformMover != null) _transformMover.Direction = _direction;
            }
            get => _direction;
        }

        [Header("Control")]
        [SerializeField]
        private Actor _actor;

        [Header("Settings")]
        [SerializeField]
        protected float _factor = 1f;
        [SerializeField]
        protected float _offset;
        [SerializeField] 
        protected AxisDirection _direction = AxisDirection.X;
        [SerializeField]
        protected AxisType _type = AxisType.Translation;
        [SerializeField] 
        protected AxisControlMode _controlMode = AxisControlMode.Position;
        [SerializeField] 
        protected UpdateLoop _updateLoop = UpdateLoop.Update;
        
        private TransformMover _transformMover;

        private void Start()
        {
            _transformMover = new TransformMover(transform, _direction, _type, AxisControlMode.Position);
        }
        
        private void OnValidate()
        {
            _transformMover?.SetConfig(_direction, _type, AxisControlMode.Position);
        }

        private void Update()
        {
            if (_updateLoop != UpdateLoop.Update) return;
            LocalUpdate(Time.deltaTime); 
        }

        private void FixedUpdate()
        {
            if (_updateLoop != UpdateLoop.FixedUpdate) return;
            LocalUpdate(Time.fixedDeltaTime); 
        }

        private void LateUpdate()
        {
            if (_updateLoop != UpdateLoop.LateUpdate) return;
            LocalUpdate(Time.deltaTime);
        }

        private void LocalUpdate(float deltaTime)
        {
            if (_actor) _target.Value = _actor.Value.Value;
            _value.Value = (_target.Value + _offset) * _factor;

            switch (_controlMode)
            {
                case AxisControlMode.Position:
                    _transformMover?.MoveTo(Value.Value);
                    break;
                case AxisControlMode.Speed:
                    _transformMover?.MoveWithSpeed(Value.Value, deltaTime);
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    public enum AxisType
    {
        Translation,
        Rotation,
        Virtual
    }

    public enum AxisControlMode
    {
        Position,
        Speed
    }
    
    public enum AxisDirection
    {
        X,
        Y,
        Z
    }

    public enum UpdateLoop
    {
        Update,
        FixedUpdate,
        LateUpdate
    }
}
