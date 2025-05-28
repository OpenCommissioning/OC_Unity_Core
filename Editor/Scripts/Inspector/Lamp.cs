using OC.VisualElements;
using OC.Interactions.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Interactions.Lamp), false), CanEditMultipleObjects]
    public class Lamp : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Interactions.Lamp;
            if (component == null) return null;
            
            var container = new VisualElement();
            
            var groupControl = new PropertyGroup("Control");
            groupControl.AddLinkOverride(serializedObject);
            groupControl.Add(new ToggleButton("Signal").BindProperty(component.Value).AlignedField()); 

            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Value", Color.green).BindProperty(component.Value).AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new ColorField("Color"){bindingPath = "_color._value"}.AlignedField());
            groupSettings.Add(new PropertyField{bindingPath = "_colorChangers"});

            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField{bindingPath = "OnValueChanged"});

            container.Add(Factory.Create(component));
            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupSettings);
            container.Add(new PropertyField{bindingPath = "_link"});
            container.Add(groupEvents);
            
            return container;
        }
    }
}