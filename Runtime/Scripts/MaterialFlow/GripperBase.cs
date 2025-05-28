using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using OC.Components;

namespace OC.MaterialFlow
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public abstract class GripperBase : Detector, IPayloadBuffer, ICustomInspector
    {
        public IPropertyReadOnly<bool> IsActive => _isActive;
        public IPropertyReadOnly<bool> IsPicked => _isPicked;

        public IReadOnlyList<Payload> Entites => _buffer;

        [SerializeField]
        private Property<bool> _isActive = new (false);
        [SerializeField]
        private Property<bool> _isPicked = new (false);
        
        public Payload.PayloadCategory PickType
        {
            get => _pickType;
            set => _pickType = value;
        }

        [SerializeField]
        private Payload.PayloadCategory _pickType;
        [SerializeField]
        private bool _dynamicSize;
        [SerializeField]
        private Vector3 _additionalColliderSize = Vector3.zero;

        public UnityEvent OnPickEvent;
        public UnityEvent OnPlaceEvent;
        public UnityEvent<bool> OnIsActiveChangedEvent;
        public UnityEvent<bool> OnIsPickedChangedEvent;
        
        private Vector3 _initColliderSize;
        private Vector3 _initColliderCenter;
        private Rigidbody _rigidbody;
        private BoxCollider _collider;
        
        [SerializeField]
        private List<Payload> _buffer = new List<Payload>();

        private new void OnEnable()
        {
            base.OnEnable();
            _isActive.OnValueChanged += OnIsActiveChanged;
            _isPicked.OnValueChanged += OnIsPickedChanged;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            _isActive.OnValueChanged += OnIsActiveChanged;
            _isPicked.OnValueChanged += OnIsPickedChanged;
        }

        private void Start()
        {
            GetReferences();
            _initColliderSize = _collider.size;
            _initColliderCenter = _collider.center;
        }

        public void Pick()
        {
            if (_isActive.Value) return;
            _isActive.Value = true;
            
            foreach (var payloadObject in _collisionDetector.Buffer)
            {
                if (payloadObject is not Payload e) continue;
                if (e.Category != _pickType) continue;
                _buffer.Add(e);
            }

            PickPayloadsAction(_buffer);
            OnPickEvent?.Invoke();
            _isPicked.Value = _buffer.Count > 0;
        }

        public void Place()
        {
            if (!_isActive.Value) return;
            _isActive.Value = false;
            PlacePayloadsAction(_buffer, GetTargetPayload());
            _buffer.Clear();
            OnPlaceEvent?.Invoke();
            _isPicked.Value = _buffer.Count > 0;
        }
        
        public void Add(Payload payload)
        {
            if (_buffer.Contains(payload)) return;
            _buffer.Add(payload);
            PickPayloadsAction(_buffer);
            OnPickEvent?.Invoke();
            _isPicked.Value = _buffer.Count > 0;
        }

        public void Remove(Payload payload)
        {
            if (!_buffer.Contains(payload)) return;
            _buffer.Remove(payload);
            _isPicked.Value = _buffer.Count > 0;
        }

        public void DeleteAll()
        {
            _collisionDetector.DestroyAll();
            _buffer.Clear();
            _isPicked.Value = _buffer.Count > 0;
        }
        
        protected abstract void PickPayloadsAction(List<Payload> payloads);
        protected abstract void PlacePayloadsAction(List<Payload> payloads, PayloadBase parent);

        private void OnIsActiveChanged(bool value)
        {
            SetColliderSize(value);
            OnIsActiveChangedEvent?.Invoke(value);
        }
        
        private void OnIsPickedChanged(bool value)
        {
            OnIsPickedChangedEvent?.Invoke(value);
        }
        private PayloadBase GetTargetPayload()
        {
            var storage = _collisionDetector.GetLastPayloadStorage();
            if (storage != null) return storage;

            switch (_pickType)
            {
                case Payload.PayloadCategory.Part:
                    var payload = _collisionDetector.GetLastByType(Payload.PayloadCategory.Assembly);
                    return payload != null ? payload : _collisionDetector.GetLastByType(Payload.PayloadCategory.Transport);
                case Payload.PayloadCategory.Assembly:
                    return _collisionDetector.GetLastByType(Payload.PayloadCategory.Transport);
                case Payload.PayloadCategory.Transport:
                    return null;
                default:
                    return null;
            }
        }

        private void SetColliderSize(bool isGripped)
        {
            if (!_dynamicSize) return;

            if (isGripped)
            {
                _collider.size = _initColliderSize + _additionalColliderSize;
                _collider.center = _initColliderCenter + _additionalColliderSize * 0.5f;
            }
            else
            {
                _collider.size = _initColliderSize;
                _collider.center = _initColliderCenter;
            }
        }

        private void GetReferences()
        {
            _collider = GetComponent<BoxCollider>();
            _rigidbody = GetComponent<Rigidbody>();
            _collider.isTrigger = true;
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }
    }
}
