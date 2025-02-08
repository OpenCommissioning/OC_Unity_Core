using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Components.DriveSimple), true), CanEditMultipleObjects]
    public class DriveSimple : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Components.DriveSimple;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            groupControl.Add(new FloatField("Target").BindProperty(component.Target).AlignedField());
            var hStack = new StackHorizontal();
            hStack.Add(new ToggleButton("Backward").BindProperty(component.Backward));
            hStack.Add(new ToggleButton("Forward").BindProperty(component.Forward));
            groupControl.Add(hStack);
            groupControl.AddForceOption(component, new IProperty[]{component.Backward, component.Forward});

            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Is Active", Color.green).BindProperty(component.IsActive).AlignedField());
            groupStatus.Add(new FloatField("Value"){isReadOnly = true}.BindProperty(component.Value).AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new FloatField("Speed"){bindingPath = "_speed._value"}.AlignedField());
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