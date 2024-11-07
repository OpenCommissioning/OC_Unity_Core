using OC.Communication;

namespace OC.Components
{
    public interface IControlOverridable 
    {
        public IProperty<bool> Override { get; }
        public Link Link { get; }
    }
}
