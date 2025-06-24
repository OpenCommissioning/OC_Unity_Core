using OC.Communication;
using UnityEngine;

namespace OC.Components
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public abstract class Device : MonoComponent, IDevice
    {
        public Link Link => _link;
        public abstract int AllocatedBitLength { get; }

        [SerializeField]
        protected Link _link;
        
        protected void Start()
        {
            _link.Initialize(this);
        }

        public virtual void Reset()
        {
            _link = new Link
            {
                Type = "FB_Device"
            };
        }
    }
}
