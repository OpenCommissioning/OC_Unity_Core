using OC.Communication;
using UnityEngine.UIElements;

namespace OC.Interactions
{
    public interface IIndustrialPanel : IDevice
    {
        public VisualElement Create();
    }
}