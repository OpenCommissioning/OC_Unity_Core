namespace OC
{
    public interface IProperty<T> : IPropertyReadOnly<T>
    {
        public new T Value { get; set; }
        public void SetValue(T value);
        public void SetValueWithoutNotify(T value);
    }
}