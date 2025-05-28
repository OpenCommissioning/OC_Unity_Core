using OC.VisualElements;
using OC.Interactions.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Interactions.Button), false), CanEditMultipleObjects]
    public class Button : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Interactions.Button;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Pressed", Color.green).BindProperty(component.Pressed).AlignedField());
            groupStatus.Add(new LampField("Feedback", Color.green).BindProperty(component.Feedback).AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new Toggle("Local Feedback"){bindingPath = "_localFeedback"}.AlignedField());
            groupSettings.Add(new EnumField("Type"){bindingPath = "_type"}.AlignedField());
            groupSettings.Add(new EnumField("Visual"){bindingPath = "_visualStyle._value"}.AlignedField());
            groupSettings.Add(new ColorField("Color"){bindingPath = "_color._value"}.AlignedField());
            groupSettings.Add(new PropertyField{bindingPath = "_colorChangers"});

            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField{bindingPath = "OnClickEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnPressedChanged"});
            groupEvents.Add(new PropertyField{bindingPath = "OnFeedbackChanged"});

            container.Add(Factory.Create(component));
            container.Add(groupStatus);
            container.Add(groupSettings);
            container.Add(new PropertyField{bindingPath = "_link"});
            container.Add(groupEvents);
            
            return container;
        }
    }
}