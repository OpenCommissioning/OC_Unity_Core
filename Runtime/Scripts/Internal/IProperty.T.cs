using System.Runtime.CompilerServices;

namespace OC
{
    public interface IProperty<T> : IPropertyReadOnly<T>
    {
        public new T Value { get; set; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetValue(T value);
    }
}