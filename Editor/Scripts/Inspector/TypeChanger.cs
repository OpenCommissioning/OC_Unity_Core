using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(MaterialFlow.TypeChanger), true), CanEditMultipleObjects]
    public class TypeChanger : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as MaterialFlow.TypeChanger;
            if (component == null) return null;
            
            var container = new VisualElement();
            
            var groupControl = new PropertyGroup("Control");
            var replaceButton = new UnityEngine.UIElements.Button{text = "Replace"};
            replaceButton.clicked += component.Replace;
            groupControl.Add(replaceButton);
            groupControl.Add(new IntegerField("Target Type ID").BindProperty(component.TargetTypeID).AlignedField());
            
            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Collision", Color.yellow).BindProperty(component.Collision).AlignedField());
            groupStatus.Add(new IntegerField("Target Type ID"){isReadOnly = true}.BindProperty(component.ActualTypeID).AlignedField());

            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new PropertyField{bindingPath = "_groupId"});
            groupSettings.Add(new PropertyField{bindingPath = "_collisionFilter"});
            
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