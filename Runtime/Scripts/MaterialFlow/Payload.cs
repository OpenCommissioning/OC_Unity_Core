using System;
using OC.Data;
using UnityEngine;

namespace OC.MaterialFlow
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class Payload : PayloadBase, IInteractable
    {
        public IProperty<ControlState> ControlState => _controlState;
        public IProperty<PhysicState> PhysicState => _physicState;
        public int TypeId => _typeId;
        public ulong UniqueId => _uniqueId;
        public override int GroupId
        {
            get => _groupId;
            set => _groupId = value;
        }
        public PayloadCategory Category => _category;
        public bool IsRegistered => _isRegistered;
        public ulong ParentUniqueId => _parentUniqueId;

        [Header("Settings")]
        [SerializeField]
        private PayloadCategory _category = PayloadCategory.Part;
        [SerializeField]
        private Property<ControlState> _controlState = new (MaterialFlow.ControlState.Ready);
        [SerializeField]
        private Property<PhysicState> _physicState = new (MaterialFlow.PhysicState.Free);
        
        [Header("Data")]
        [SerializeField]
        private int _typeId;
        [SerializeField]
        private ulong _uniqueId;
        [SerializeField]
        private int _groupId;
        [SerializeField]
        private bool _isRegistered;
        [SerializeField] 
        private ulong _parentUniqueId;

        private Rigidbody _rigidbody;
        private Collider _collider;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }
        
        private void OnEnable()
        {
            Pool.Instance.PoolManager.Registrate(this);
            RegistrateInParent();
            
            _controlState.ValueChanged += OnControlStateChanged;
            _physicState.ValueChanged += OnPhysicStateChanged;
            
            OnPhysicStateChanged(_physicState);
        }

        private new void OnDisable()
        {
            base.OnDisable();

            Pool.Instance.PoolManager.Unregistrate(this);
            UnregistrateInParent();
            
            _controlState.ValueChanged -= OnControlStateChanged;
            _physicState.ValueChanged -= OnPhysicStateChanged;
        }

        private new void OnDestroy()
        {
            base.OnDestroy();
            
            Pool.Instance.PoolManager.Unregistrate(this);
            UnregistrateInParent();
        }

        public void Registrate(ulong uniqueId = 0)
        {
            _isRegistered = true;
            _uniqueId = uniqueId;
        }

        public void Unregistrate(ulong uniqueId = 0)
        {
            _isRegistered = false;
            _uniqueId = uniqueId;
        }

        public void ApplyDiscription(PayloadDescription description)
        {
            Unregistrate(description.UniqueId);
            name = description.Name;
            _controlState.Value = (ControlState)description.ControlState;
            _physicState.Value = (PhysicState)description.PhysicState;
            _category = (PayloadCategory)description.Type;
            _groupId = description.GroupId;
            _typeId = description.TypeId;
            _parentUniqueId = description.ParentUniqueId;
        }

        private void OnControlStateChanged(ControlState state)
        {
            
        }
        
        private void OnPhysicStateChanged(PhysicState state)
        {
            _rigidbody ??= GetComponent<Rigidbody>();
            _collider ??= GetComponent<Collider>();

            switch (state)
            {
                case MaterialFlow.PhysicState.Free:
                    _rigidbody.useGravity = true;
                    _rigidbody.isKinematic = false;
                    _collider.isTrigger = false;
                    break;
                case MaterialFlow.PhysicState.Parent:
                    _rigidbody.isKinematic = true;
                    _collider.isTrigger = true;
                    break;
                case MaterialFlow.PhysicState.Static:
                    _rigidbody.isKinematic = true;
                    _collider.isTrigger = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void RegistrateInParent()
        {
            var payloadBuffer = gameObject.GetComponentInParent<IPayloadBuffer>();
            payloadBuffer?.Add(this);
        }

        private void UnregistrateInParent()
        {
            var payloadBuffer = gameObject.GetComponentInParent<IPayloadBuffer>();
            payloadBuffer?.Remove(this);
        }

        private void OnJointBreak(float breakForce)
        {
            if (ControlState.Value == MaterialFlow.ControlState.Busy) ControlState.Value = MaterialFlow.ControlState.Ready;
        }

        public void SetParent(Transform parent)
        {
            UnregistrateInParent();
            if (parent == null)
            {
                transform.parent = Pool.Instance.transform;
                PhysicState.Value = MaterialFlow.PhysicState.Free;
            }
            else
            {
                transform.parent = parent;
                PhysicState.Value = MaterialFlow.PhysicState.Parent;
                RegistrateInParent();
            }
        }

        public Payload GetParentPayload()
        {
            if (transform.parent is null) return null;
            return transform.parent.TryGetComponent(out Payload payload) ? payload : null;
        }
        
        public enum PayloadCategory
        {
            Part,
            Assembly,
            Transport
        }

        public override string ToString() => $"{gameObject.name} (UniqueId: {_uniqueId}, TypeId: {_typeId})";
    }

    public enum ControlState
    {
        Ready,
        Busy,
        Done,
        Error
    }
        
    public enum PhysicState
    {
        Free,
        Parent,
        Static
    }
}
