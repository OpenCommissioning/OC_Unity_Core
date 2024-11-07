using UnityEngine;

namespace OC.Components
{
    public abstract class Actor : MonoComponent, IMeasurement<float>
    {
        public IProperty<bool> Override => _override;
        public IProperty<float> Target => _target;
        public IPropertyReadOnly<float> Value => _value;

        [SerializeField]
        protected Property<bool> _override = new (false);
        [SerializeField]
        protected Property<float> _target = new (0);
        [SerializeField]
        protected Property<float> _value = new (0);
    }
}
