using OC.Communication;
using UnityEngine;

namespace OC.Components
{
    [DefaultExecutionOrder(100)]
    public abstract class DeviceWord : MonoComponent, IDevice
    {
        public Link Link => _link;
        public ConnectorDataWord Connector => _connector;
        public abstract int AllocatedBitLength { get; }

        [SerializeField]
        protected Link _link; 
        private ConnectorDataWord _connector;
        
        protected void Start()
        {
            _link.Initialize(this);
            _connector = new ConnectorDataWord(Link);
        }
        
        private void Reset()
        {
            _link = new Link(this, "FB_DeviceWord");
        }
    }
}
