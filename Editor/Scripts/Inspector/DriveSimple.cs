using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Components.DriveSimple), false), CanEditMultipleObjects]
    public class DriveSimple : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Components.DriveSimple;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            groupControl.AddOverride(serializedObject);
            var hStack = new StackHorizontal();
            hStack.Add(new ToggleButton("Backward").BindProperty(component.Backward));
            hStack.Add(new ToggleButton("Forward").BindProperty(component.Forward));
            groupControl.Add(hStack);

            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Is Active", Color.green){bindingPath = "_stateObserver._isActive._value"}.AlignedField());
            groupStatus.Add(new FloatField("Value"){isReadOnly = true, bindingPath = "_value._value"}.AlignedField());
            
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