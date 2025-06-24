using OC.Components;
using UnityEngine;

namespace OC.MaterialFlow
{
    [SelectionBase]
    [RequireComponent(typeof(BoxCollider))]
    [DisallowMultipleComponent]
    [AddComponentMenu("Open Commissioning/Material Flow/Sink")]
    public class Sink : Detector, ICustomInspector, IInteractable
    {
        public Property<bool> Auto => _auto;
        
        [SerializeField]
        protected Property<bool> _auto = new (false);

        private new void OnEnable()
        {
            base.OnEnable();
            GetComponent<BoxCollider>().isTrigger = true;
        }
        
        private void OnValidate()
        {
            _auto.OnValidate();
        }
        
        private void FixedUpdate()
        {
            if (_auto) Delete();
        }

        public void Delete() => DestroyAll();
    }
}
