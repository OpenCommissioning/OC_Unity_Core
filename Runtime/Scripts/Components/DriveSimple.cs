using UnityEngine;
using OC.Communication;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Actor/Drive Simple")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class DriveSimple : DriveSpeed
    {
        public IProperty<float> Speed => _speed;

        public Property<bool> Forward
        {
            get => _forward;
            set
            {
                _forward = value;
                if (_forward) _backward.Value = false;
            }
        }
        
        public Property<bool> Backward
        {
            get => _backward;
            set
            {
                _backward = value;
                if (_backward) _forward.Value = false;
            }
        }

        public bool JogForward
        {
            get => Forward.Value;
            set => Forward.Value = value;
        }
        
        public bool JogBackward
        {
            get => Backward.Value;
            set => Backward.Value = value;
        }

        [SerializeField]
        private Property<float> _speed = new (100);
        [SerializeField]
        private Property<bool> _forward = new (false);
        [SerializeField]
        private Property<bool> _backward = new (false);

        protected override void GetLinkData()
        {
            _forward.Value = _connectorData.Control.GetBit(0);
            _backward.Value = _connectorData.Control.GetBit(1);
        }

        protected override void Operation(float deltaTime)
        {
            if (_forward.Value ^ _backward.Value)
            {
                _target.Value = _forward.Value ? _speed.Value : -_speed.Value;
            }
            else
            {
                _target.Value = 0;
            }
            base.Operation(deltaTime);
        }
    }
}