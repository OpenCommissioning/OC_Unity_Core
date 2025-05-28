using OC.VisualElements;
using OC.MaterialFlow;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(MaterialFlow.Source), false), CanEditMultipleObjects]
    public class Source : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as MaterialFlow.Source;
            if (component == null) return null;
            
            var container = new VisualElement();
            
            var groupControl = new PropertyGroup("Control");
            var createButton = new UnityEngine.UIElements.Button{text = "Create"};
            var deleteButton = new UnityEngine.UIElements.Button{text = "Delete"};
            createButton.clicked += component.Create;
            deleteButton.clicked += component.Delete;
            groupControl.Add(createButton);
            groupControl.Add(deleteButton);
            
            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Collision", Color.yellow).BindProperty(component.Collision).AlignedField());

            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new IntegerField("Type ID"){bindingPath = "_typeId._value"}.AlignedField());
            groupSettings.Add(new LongField("Unique ID"){bindingPath = "_uniqueId._value"}.AlignedField());
            groupSettings.Add(new Toggle("Auto Mode"){bindingPath = "_auto._value"}.AlignedField());
            groupSettings.Add(new IntegerField("Group ID"){bindingPath = "_groupId"}.AlignedField());
            groupSettings.Add(new MaskField("Collision Filter"){ choices = Detector.Filter, bindingPath = "_collisionFilter"}.AlignedField());
            
            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField{bindingPath = "OnCollisionEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnPayloadEnterEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnPayloadExitEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnPayloadCreated"});
            
            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupSettings);
            container.Add(groupEvents);
            
            return container;
        }
    }
}