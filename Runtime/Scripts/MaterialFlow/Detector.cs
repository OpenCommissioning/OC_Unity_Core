using System;
using System.Collections.Generic;
using OC.Components;
using UnityEngine;
using UnityEngine.Events;

namespace OC.MaterialFlow
{
    [DisallowMultipleComponent]
    public class Detector : MonoComponent 
    {
        public IPropertyReadOnly<bool> Collision => _collision;

        public IList<PayloadBase> CollisionBuffer => _collisionBuffer;
        
        public CollisionFilter CollisionFilter
        {
            get => _collisionFilter;
            set => _collisionFilter = value;
        }

        public int GroupId
        {
            get => _groupId;
            set => _groupId = value;
        }
        
        [SerializeField]
        protected Property<bool> _collision = new (false);
        [SerializeField]
        protected int _groupId;
        [SerializeField]
        protected CollisionFilter _collisionFilter = CollisionFilter.All;

        private readonly List<PayloadBase> _collisionBuffer = new ();
        
        public UnityEvent<bool> OnCollisionEvent;
        public UnityEvent<PayloadBase> OnPayloadEnterEvent;
        public UnityEvent<PayloadBase> OnPayloadExitEvent;
        public Action<PayloadBase> PayloadEnterAction;
        public Action<PayloadBase> PayloadExitAction;

        protected void OnEnable()
        {
            _collision.OnValueChanged += OnCollisionChangedAction;
        }

        protected void OnDisable()
        {
            _collision.OnValueChanged -= OnCollisionChangedAction;
        }

        protected void OnTriggerEnter(Collider other)
        {
            Add(other.attachedRigidbody == null ? other.gameObject : other.attachedRigidbody.gameObject);
        }
        
        protected void OnTriggerExit(Collider other)
        {
            Remove(other.attachedRigidbody == null ? other.gameObject : other.attachedRigidbody.gameObject);
        }

        protected virtual void OnCollisionChangedAction(bool value)
        {
            _collision.Value = value;
            OnCollisionEvent?.Invoke(value);
        }

        public static readonly List<string> Filter = new ()
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

        public void Add(GameObject target)
        {
            if (target.TryGetComponent(out PayloadBase payloadObject))
            {
                Add(payloadObject);
            }
        }
        
        public void Add(PayloadBase payloadBase)
        {
            if (!PayloadUtils.IsTypeValid(payloadBase, _collisionFilter)) return;
            if (!PayloadUtils.IsGroupValid(payloadBase.GroupId, _groupId)) return;
            if (_collisionBuffer.Contains(payloadBase)) return;

            payloadBase.OnDestroyAction += () => Remove(payloadBase);
            payloadBase.OnDisableAction += () => Remove(payloadBase);
            _collisionBuffer.Add(payloadBase);
            OnPayloadEnterAction(payloadBase);
            Refresh();
        }
        
        public void Remove(GameObject target)
        {
            if (target.TryGetComponent(out PayloadBase payloadObject))
            {
                Remove(payloadObject);
            }
        }
        
        public void Remove(PayloadBase payloadBase)
        {
            if (!PayloadUtils.IsTypeValid(payloadBase, _collisionFilter) || !PayloadUtils.IsGroupValid(payloadBase.GroupId, _groupId)) return;
            if (!_collisionBuffer.Contains(payloadBase)) return;
            _collisionBuffer.Remove(payloadBase);
            OnPayloadExitAction(payloadBase);
            Refresh();
        }
        
        public void ClearAll()
        {
            _collisionBuffer.Clear();
            Refresh();
        }
        
        public void DestroyAll()
        {
            var destroyList = new List<PayloadBase>(_collisionBuffer);
            
            foreach (var payload in destroyList)
            {
                if (!Application.isPlaying)
                {
                    DestroyImmediate(payload.gameObject);
                }
                else
                {
                    Destroy(payload.gameObject);
                }
            }
            ClearAll();
        }

        protected virtual void OnPayloadEnterAction(PayloadBase payloadBase)
        {
            PayloadEnterAction?.Invoke(payloadBase);
            OnPayloadEnterEvent?.Invoke(payloadBase);
        }
        
        protected virtual void OnPayloadExitAction(PayloadBase payloadBase)
        {
            PayloadExitAction?.Invoke(payloadBase);
            OnPayloadExitEvent?.Invoke(payloadBase);
        }
        
        private void Refresh()
        {
            _collision.Value = _collisionBuffer.Count > 0;
        }
    }
}