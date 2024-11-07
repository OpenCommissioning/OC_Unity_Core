using OC.Communication;
using UnityEngine;

namespace OC.Components
{
    [DefaultExecutionOrder(100)]
    public abstract class DeviceDWord : MonoComponent, IDevice
    {
        public Link Link => _link;
        public ConnectorDataDWord ConnectorData => _connectorData;
        public abstract int AllocatedBitLength { get; }

        [SerializeField]
        protected Link _link; 
        private ConnectorDataDWord _connectorData;
        
        protected void Start()
        {
            _link.Initialize(this);
            _connectorData = new ConnectorDataDWord(Link);
        }
        
        private void Reset()
        {
            _link = new Link(this, "FB_DeviceDWord");
        }
    }
}
