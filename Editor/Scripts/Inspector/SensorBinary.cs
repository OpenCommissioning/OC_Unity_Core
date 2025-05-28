using OC.VisualElements;
using OC.MaterialFlow;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Components.SensorBinary), true), CanEditMultipleObjects]
    public class SensorBinary : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Components.SensorBinary;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            groupControl.AddLinkOverride(serializedObject);
            groupControl.Add(new ToggleButton("Collision").BindProperty(component.State).AlignedField());
            
            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Collision", Color.yellow).BindProperty(component.Collision).AlignedField());
            groupStatus.Add(new LampField("Value", Color.green).BindProperty(component.Value).AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new IntegerField("Group ID"){ bindingPath = "_groupId"}.AlignedField());
            groupSettings.Add(new MaskField("Collision Filter"){choices = Detector.Filter, bindingPath = "_collisionFilter"}.AlignedField());
            groupSettings.Add(new FloatField("Length"){bindingPath = "_length._value"}.AlignedField());
            groupSettings.Add(new Toggle("Invert"){bindingPath = "_invert"}.AlignedField());
            groupSettings.Add(new Toggle("Use Box Collider"){bindingPath = "_useBoxCollider"}.AlignedField());

            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField{bindingPath = "OnValueChangedEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnCollisionEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnPayloadEnterEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnPayloadExitEvent"});

            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupSettings);
            container.Add(new PropertyField{bindingPath = "_link"});
            container.Add(groupEvents);
            
            return container;
        }
    }
}