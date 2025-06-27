using OC.Communication;
using OC.MaterialFlow;
using UnityEngine;
using UnityEngine.Events;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Sensor/Sensor Binary")]
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class SensorBinary : Detector, IDevice, IMeasurement<bool>, ISensorBeam, ICustomInspector, IInteractable
    {
        public Link Link => _link;
        public IProperty<bool> Override => _override;
        public IPropertyReadOnly<bool> Value => _value;
        public IProperty<bool> State => _state;
        public IPropertyReadOnly<float> Length => _length;

        public bool Invert
        {
            get => _invert;
            set => _invert = value;
        }
        
        [SerializeField]
        protected Property<bool> _override = new (false);
        [SerializeField]
        protected Property<bool> _value = new (false);
        [SerializeField]
        protected Property<bool> _state = new (false);
        [SerializeField]
        protected Property<float> _length = new (1);
        [SerializeField] 
        private bool _invert;
        [SerializeField]
        private bool _useBoxCollider;
        
        public UnityEvent<bool> OnValueChangedEvent;
        
        private const float DIAMETER = 0.004f;
        
        private BoxCollider _collider;
        private Rigidbody _rigidbody;

        [SerializeField]
        private Link _link;

        private new void OnEnable()
        {
            base.OnEnable();
            _state.Subscribe(OnStateChanged);
            _value.Subscribe(OnValueChanged);
            //_state.OnValueChanged += OnStateChanged;
            //_value.OnValueChanged += OnValueChanged;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            _state.Unsubscribe(OnStateChanged);
            _value.Unsubscribe(OnValueChanged);
            //_state.OnValueChanged -= OnStateChanged;
            //_value.OnValueChanged -= OnValueChanged;
        }
        
        private void Start()
        {
            Link.Initialize(this);
            Initialize();
        }

        public void Reset()
        {
            _link = new Link
            {
                Type = "FB_SensorBinary"
            };
        }

        private void OnValidate()
        {
            Initialize();
            OnStateChanged(_state.Value);
            _length.OnValidate();
        }

        [ContextMenu("Bound Box Collider Size", false, 100)]
        public new void BoundBoxColliderSize()
        {
            if (Utils.TryBoundBoxColliderSize(gameObject, out _))
            {
                _useBoxCollider = true;
            }
        }

        protected override void OnCollisionChangedAction(bool value)
        {
            base.OnCollisionChangedAction(value);
            if (_override) return;
            _state.Value = value;
        }

        private void OnStateChanged(bool value)
        {
            _value.Value = _invert ? !value : value;
        }
        
        private void OnValueChanged(bool value)
        {
            Link.Status.SetBit(0, value);
            OnValueChangedEvent?.Invoke(value);
        }

        private void Initialize()
        {
            if (_collider == null) _collider = GetComponent<BoxCollider>();
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
            
            _collider.isTrigger = true;
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;

            if (_useBoxCollider) return;
            _collider.size = new Vector3(DIAMETER, DIAMETER, _length);
            _collider.center = new Vector3(0, 0, _length * 0.5f);
        }
    }
}
