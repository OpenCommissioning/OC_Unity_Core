using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Components.DriveSpeed), true), CanEditMultipleObjects]
    public class DriveSpeed : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Components.DriveSpeed;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            groupControl.AddLinkOverride(serializedObject);
            groupControl.Add(new FloatField("Target").BindProperty(component.Target).AlignedField());

            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Is Active", Color.green).BindProperty(component.IsActive).AlignedField());
            groupStatus.Add(new FloatField("Value"){isReadOnly = true}.BindProperty(component.Value).AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new FloatField("Acceleration"){bindingPath = "_acceleration._value"}.AlignedField());

            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField{bindingPath = "OnActiveChanged"});
            
            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupSettings);
            container.Add(new PropertyField{bindingPath = "_link"});
            container.Add(groupEvents);
            
            return container;
        }
    }
}