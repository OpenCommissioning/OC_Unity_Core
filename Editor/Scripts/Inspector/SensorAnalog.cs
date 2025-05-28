using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Components.SensorAnalog), true), CanEditMultipleObjects]
    public class SensorAnalog : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Components.SensorAnalog;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            groupControl.AddLinkOverride(component);
            groupControl.Add(new FloatField("Value").BindProperty(component.Value).AlignedField());
            
            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new FloatField("Value"){isReadOnly = true}.BindProperty(component.Value).AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new PropertyField{bindingPath = "_factor"});
            groupSettings.Add(new PropertyField{bindingPath = "_valueSource"});

            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField {bindingPath = "OnValueChangedEvent"});

            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupSettings);
            container.Add(new PropertyField{bindingPath = "_link"});
            container.Add(groupEvents);
            
            return container;
        }
    }
}