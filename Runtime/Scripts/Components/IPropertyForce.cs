using OC.Communication;

namespace OC.Components
{
    public interface IPropertyForce: IConnectable
    {
        public IProperty<bool> Force { get; }
    }
}
