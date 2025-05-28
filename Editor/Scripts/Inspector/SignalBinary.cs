using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Components.SignalBinary), true), CanEditMultipleObjects]
    public class SignalBinary : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Components.SignalBinary;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            groupControl.AddLinkOverride(serializedObject);
            groupControl.Add(new ToggleButton("Signal").BindProperty(component.Signal).AlignedField());
            
            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Value", Color.green).BindProperty(component.Value).AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new PropertyField{bindingPath = "_device"});

            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField{bindingPath = "OnValueChangedEvent"});

            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupSettings);
            container.Add(new PropertyField{bindingPath = "_link"});
            container.Add(groupEvents);
            
            return container;
        }
    }
}