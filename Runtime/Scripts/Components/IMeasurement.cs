namespace OC.Components
{
    public interface IMeasurement<T> 
    {
        public IPropertyReadOnly<T> Value { get; }
    }
}

