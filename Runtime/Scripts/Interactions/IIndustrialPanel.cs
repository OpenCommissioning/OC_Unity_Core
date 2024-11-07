using OC.Communication;
using UnityEngine.UIElements;

namespace OC.Interactions
{
    public interface IIndustrialPanel
    {
        public Link Link { get; }
        public VisualElement Create();
    }
}