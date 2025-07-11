using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Components.DrivePosition), false), CanEditMultipleObjects]
    public class DrivePosition : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Components.DrivePosition;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            groupControl.AddOverride(serializedObject);
            groupControl.Add(new FloatField("Target"){bindingPath = "_target._value"}.AlignedField());
            
            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Is Active", Color.green){bindingPath = "_stateObserver._isActive._value"}.AlignedField());
            groupStatus.Add(new FloatField("Value"){isReadOnly = true, bindingPath = "_value._value"}.AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new FloatField("Speed"){bindingPath = "_speed._value"}.AlignedField());

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