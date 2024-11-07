using OC.VisualElements;
using OC.MaterialFlow;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(MaterialFlow.Sink), true), CanEditMultipleObjects]
    public class Sink : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as MaterialFlow.Sink;
            if (component == null) return null;
            
            var container = new VisualElement();
            
            var groupControl = new PropertyGroup("Control");
            var deleteButton = new UnityEngine.UIElements.Button{text = "Delete"};
            deleteButton.clicked += component.Delete;
            groupControl.Add(deleteButton);
            
            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Collision", Color.yellow).BindProperty(component.Collision).AlignedField());

            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new Toggle("Auto Mode").BindProperty(component.Auto).AlignedField());
            groupSettings.Add(new IntegerField("Group ID"){bindingPath = "_groupId"}.AlignedField());
            groupSettings.Add(new MaskField("Collision Filter"){choices = Detector.Filter, bindingPath = "_collisionFilter"}.AlignedField());
            
            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField{bindingPath = "OnCollisionEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnPayloadEnterEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnPayloadExitEvent"});
            
            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupSettings);
            container.Add(groupEvents);
            
            return container;
        }
    }
}