namespace OC.Components
{
    public interface ISensorBeam
    {
        public IProperty<bool> State { get; }
        public IPropertyReadOnly<float> Length { get; }
    }
}
