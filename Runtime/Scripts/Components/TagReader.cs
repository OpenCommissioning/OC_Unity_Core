using UnityEngine;
using OC.Communication;
 using OC.MaterialFlow;
 using UnityEngine.Events;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Sensor/Payload Tag Reader")]
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class TagReader : Detector, IDevice, IMeasurement<ulong>, IControlOverridable, ICustomInspector, IInteractable
    {
        public Link Link => _link;
        public IProperty<bool> Override => _override;
        public IPropertyReadOnly<ulong> Value => _value;

        [SerializeField]
        protected Property<bool> _override = new (false);
        [SerializeField]
        protected Property<ulong> _value = new (0);

        [SerializeField]
        private bool _holdValue;

        public UnityEvent<ulong> OnValueChangedEvent;

        [SerializeField]
        private Link _link;
        private ConnectorDataLWord _connectorData;
        
        private BoxCollider _collider;
        private Rigidbody _rigidbody;
        
        private void Awake()
        {
            gameObject.layer = (int)DefaultLayers.Reader;
        }
        
        private new void OnEnable()
        {
            base.OnEnable();
            _value.ValueChanged += OnValueChanged;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            _value.ValueChanged -= OnValueChanged;
        }

        private void Start()
        {
            _link.Initialize(this);
            _connectorData = new ConnectorDataLWord(_link);
            Initialize();
        }

        private void Reset()
        {
            _link = new Link(this, "FB_Reader");
        }

        protected override void OnPayloadEnterAction(PayloadBase payloadBase)
        {
            if (payloadBase is not Payload payload) return;
            OnPayloadEnterEvent?.Invoke(payload);
            if (!payload.TryGetComponent(out PayloadTag payloadTag)) return;
            if (!_override.Value) _value.Value = payloadTag.Read();
        }

        protected override void OnPayloadExitAction(PayloadBase payloadBase)
        {
            if (payloadBase is not Payload payload) return;
            OnPayloadExitEvent?.Invoke(payload);
            if (_override.Value) return;
            if (_holdValue) return;
            _value.Value = 0;
        }
        
        private void OnValueChanged(ulong value)
        {
            OnValueChangedEvent?.Invoke(value);
            _connectorData.StatusData = (long)value;
        }
        
        private void Initialize()
        {
            if (_collider == null) _collider = GetComponent<BoxCollider>();
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
            
            _collider.isTrigger = true;
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }
    }
}
