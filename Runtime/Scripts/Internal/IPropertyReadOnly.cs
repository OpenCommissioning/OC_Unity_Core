using System;

namespace OC
{
    public interface IPropertyReadOnly<T> : IProperty
    {
        public T Value { get; }
        
        public event Action<T> ValueChanged;
        public void ForceSetValue(T value);
        public void OnValidate();
    }
}