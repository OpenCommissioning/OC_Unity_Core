using System.Collections.Generic;
using OC.Components;
using UnityEngine;
using UnityEngine.Events;

namespace OC.MaterialFlow
{
    [DisallowMultipleComponent]
    public abstract class Detector : MonoComponent 
    {
        public IPropertyReadOnly<bool> Collision => _collision;
        
        [SerializeField]
        protected Property<bool> _collision = new (false);

        [SerializeField]
        protected int _groupId;
        [SerializeField]
        protected CollisionFilter _collisionFilter = CollisionFilter.All;

        protected readonly CollisionDetector _collisionDetector = new(CollisionFilter.All, 0);

        public UnityEvent<bool> OnCollisionEvent;
        public UnityEvent<PayloadBase> OnPayloadEnterEvent;
        public UnityEvent<PayloadBase> OnPayloadExitEvent;
        
        protected void OnEnable()
        {
            _collisionDetector.Filter = _collisionFilter;
            _collisionDetector.GroupId = _groupId;
            _collisionDetector.Collision.ValueChanged += OnCollisionChangedAction;
            _collisionDetector.PayloadEnterAction += OnPayloadEnterAction;
            _collisionDetector.PayloadExitAction += OnPayloadExitAction;
        }
        
        protected void OnDisable()
        {
            _collisionDetector.Collision.ValueChanged -= OnCollisionChangedAction;
            _collisionDetector.PayloadEnterAction -= OnPayloadEnterAction;
            _collisionDetector.PayloadExitAction -= OnPayloadExitAction;
        }
        
        protected void OnValidate()
        {
            _collisionDetector.Filter = _collisionFilter;
            _collisionDetector.GroupId = _groupId;
        }
        
        protected void OnTriggerEnter(Collider other)
        {
            _collisionDetector.Add(other.attachedRigidbody == null
                ? other.gameObject
                : other.attachedRigidbody.gameObject);
        }
        
        protected void OnTriggerExit(Collider other)
        {
            _collisionDetector.Remove(other.attachedRigidbody == null
                ? other.gameObject
                : other.attachedRigidbody.gameObject);
        }

        protected virtual void OnCollisionChangedAction(bool value)
        {
            _collision.Value = value;
            OnCollisionEvent?.Invoke(value);
        }

        protected virtual void OnPayloadEnterAction(PayloadBase payloadBase)
        {
            OnPayloadEnterEvent?.Invoke(payloadBase);
        }

        protected virtual void OnPayloadExitAction(PayloadBase payloadBase)
        {
            OnPayloadExitEvent?.Invoke(payloadBase);
        }

        public static readonly List<string> Filter = new List<string>()
        {
            "Part",
            "Assembly",
            "Transport",
            "Static",
            "Storage"
        };
        
        [ContextMenu("Bound Box Collider Size", false, 100)]
        public void BoundBoxColliderSize()
        {
            Utils.TryBoundBoxColliderSize(gameObject, out _);
        }
    }
}