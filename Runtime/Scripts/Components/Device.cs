using OC.Communication;
using UnityEngine;

namespace OC.Components
{
    public abstract class Device : MonoComponent, IDevice
    {
        public Link Link => _link;
        public Connector Connector => _connector;
        public abstract int AllocatedBitLength { get; }

        [SerializeField]
        protected Link _link;
        private Connector _connector;
        
        protected void Start()
        {
            _link.Initialize(this);
            _connector = new Connector(Link);
        }

        protected virtual void Reset()
        {
            _link = new Link(this, "FB_Device");
        }
    }
}
