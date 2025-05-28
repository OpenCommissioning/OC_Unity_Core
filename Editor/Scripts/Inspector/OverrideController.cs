using OC.Communication;
using OC.VisualElements;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class OverrideController : ToggleButton
    {
        private readonly VisualElement _target;
        private bool _override;
        
        public OverrideController(IDevice device, VisualElement target)
        {
            _target = target;
            label = "Override";

            this.BindProperty(device.Link.Override);
            
            device.Link.Override.Subscribe(OnOverrideChanged);
        }

        private void OnOverrideChanged(bool @override)
        {
            _target.SetEnabled(@override);
        }
    }
}