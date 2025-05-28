using OC.Components;
using UnityEngine;

namespace OC.MaterialFlow
{
    [SelectionBase]
    [RequireComponent(typeof(BoxCollider))]
    [DisallowMultipleComponent]
    [AddComponentMenu("Open Commissioning/Material Flow/Type Changer")]
    public class TypeChanger : Detector, ICustomInspector, IInteractable
    {
        public IProperty<int> TargetTypeID => _targetTypeId;
        public IProperty<int> ActualTypeID => _actualTypeId;

        [SerializeField]
        // ReSharper disable once NotAccessedField.Local
        private Property<int> _actualTypeId = new(-1) ;
        [SerializeField]
        private Property<int> _targetTypeId;  

        private Payload _payload;

        private new void OnEnable()
        {
            base.OnEnable();
            OnPayloadEnterEvent.AddListener(OnPayloadEnter);
            OnPayloadExitEvent.AddListener(OnPayloadExit);
            GetComponent<BoxCollider>().isTrigger = true;
        }
        
        private new void OnDisable()
        {
            base.OnDisable();
            OnPayloadEnterEvent.RemoveListener(OnPayloadEnter);
            OnPayloadExitEvent.RemoveListener(OnPayloadExit);
        }

        private void OnPayloadEnter(PayloadBase payloadBase)
        {
            if (payloadBase is not Payload payload) return;
            _payload = payload;
            _actualTypeId.Value = _payload.TypeId;
        }
        
        private void OnPayloadExit(PayloadBase payloadBase)
        {
            if (payloadBase is not Payload payload) return;
            if (_payload != payload) return;
            _payload = null;
            _actualTypeId.Value = -1;
        }

        public void Replace()
        {
            Replace(_targetTypeId);
        }

        public void Replace(int typeId)
        {
            Pool.Instance.PoolManager.Replace(this, _payload, typeId);
        }
    }
}
