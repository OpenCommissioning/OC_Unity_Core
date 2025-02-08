using System;

namespace OC
{
    public interface IPropertyReadOnly<T> : IProperty
    {
        public T Value { get; }
        public void OnValidate();
        public event Action<T> OnValueChanged;
    }
}