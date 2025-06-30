using OC.Communication;
using UnityEngine;

namespace OC.Components
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public abstract class SampleDevice : MonoComponent, IDevice
    {
        public abstract Link Link { get; }
        public IProperty<bool> Override => _override;
        public abstract int AllocatedBitLength { get; }

        [SerializeField]
        protected Property<bool> _override = new (false);
    }
}
