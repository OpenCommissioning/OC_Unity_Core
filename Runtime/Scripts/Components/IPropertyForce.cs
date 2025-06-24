using OC.Communication;

namespace OC.Components
{
    public interface IPropertyForce: ILink
    {
        public IProperty<bool> Force { get; }
    }
}
