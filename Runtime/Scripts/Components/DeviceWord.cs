using OC.Communication;
using UnityEngine;

namespace OC.Components
{
    [DefaultExecutionOrder(100)]
    public abstract class DeviceWord : MonoComponent, IDevice
    {
        public Link Link => _link;
        public abstract int AllocatedBitLength { get; }

        [SerializeField]
        protected LinkDataWord _link; 
        
        protected void Start()
        {
            _link.Initialize(this);
        }
        
        private void Reset()
        {
            _link = new LinkDataWord
            {
                Type = "FB_DeviceWord"
            };
        }
    }
}
