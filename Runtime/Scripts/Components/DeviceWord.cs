using OC.Communication;
using UnityEngine;

namespace OC.Components
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    public abstract class DeviceWord : MonoComponent, IDevice
    {
        public Link Link => _link;
        public IProperty<bool> Override => _override;
        public abstract int AllocatedBitLength { get; }

        [SerializeField]
        protected Property<bool> _override = new (false);
        [SerializeField]
        protected LinkDataWord _link; 
        
        protected void Start()
        {
            _link.Initialize(this);
        }
        
        public void Reset()
        {
            _link = new LinkDataWord
            {
                Type = "FB_DeviceWord"
            };
        }
    }
}
