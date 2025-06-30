using OC.Components;
using UnityEngine.UIElements;

namespace OC.Interactions.UIElements
{
    public static class Factory 
    {
        public static VisualElement Create(SampleDevice interactionSampleDevice)
        {
            if (interactionSampleDevice == null) return null;
            
            switch (interactionSampleDevice)
            {
                case Interactions.Button target:
                {
                    var visualElement = new Button(target.name);
                    visualElement.Bind(target);
                    return visualElement;
                }
                case Interactions.Lamp target:
                {
                    var visualElement = new Lamp(target.name);
                    visualElement.Bind(target);
                    return visualElement;
                }
                case Interactions.SwitchRotary target:
                {
                    var visualElement = new SwitchRotary(target.name);
                    visualElement.Bind(target);
                    return visualElement;
                }
                default:
                    return null;
            }
        }
    }
}