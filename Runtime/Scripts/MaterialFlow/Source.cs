using System;
using OC.Components;
using UnityEngine;
using UnityEngine.Events;

namespace OC.MaterialFlow
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [AddComponentMenu("Open Commissioning/Material Flow/Source")]
    [RequireComponent(typeof(BoxCollider))]
    [Serializable]
    public class Source : Detector, ISource, ICustomInspector, IInteractable
    {
        public Property<bool> Auto => _auto;
        public Property<int> TypeId => _typeId;
        public Property<ulong> UniqueId => _uniqueId;

        [SerializeField] 
        protected Property<bool> _auto = new (false);
        [SerializeField]
        protected Property<int> _typeId = new (0);
        [SerializeField]
        protected Property<ulong> _uniqueId;

        [SerializeField]
        protected int _sourceId;

        public UnityEvent<Payload> OnPayloadCreated;
        public event Action<Payload> OnPayloadCreatedAction;

        private new void OnEnable()
        {
            base.OnEnable();
            GetComponent<BoxCollider>().isTrigger = true;
        }
        
        private new void OnValidate()
        {
            base.OnValidate();
            _typeId.OnValidate();
        }

        private void FixedUpdate()
        {
            if (!_auto.Value) return;
            Create();
        }

        private int TypeIdValidate(int value)
        {
            return Mathf.Clamp(value, 0, Pool.Instance.PoolManager.PayloadList.Count);
        }

        public virtual void Create()
        {
            if (_collisionDetector.Collision) return;

            try
            {
                var payload = Pool.Instance.PoolManager.Instantiate(this);
                if (payload == null) return;
                OnPayloadCreatedAction?.Invoke(payload);
                OnPayloadCreated?.Invoke(payload);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public virtual void Delete()
        {
            _collisionDetector.DestroyAll();
        }
    }
}
