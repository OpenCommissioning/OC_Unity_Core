using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Components.DataReader), true), CanEditMultipleObjects]
    public class DataReader : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Components.DataReader;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            var readButton = new UnityEngine.UIElements.Button{text = "Read"};
            var writeButton = new UnityEngine.UIElements.Button{text = "Write"};
            var clearButton = new UnityEngine.UIElements.Button{text = "Clear"};
            readButton.clicked += component.Read;
            writeButton.clicked += component.Write;
            clearButton.clicked += component.Clear;
            groupControl.Add(readButton);
            groupControl.Add(writeButton);
            groupControl.Add(clearButton);
            groupControl.Add(new TextField("Target Data").BindProperty(component.TargetData));
            
            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Collision", Color.yellow).BindProperty(component.Collision).AlignedField());
            groupStatus.Add(new TextField("Raw Data"){isReadOnly = true}.BindProperty(component.RawData).AlignedField());
            groupStatus.Add(new FloatField("Value"){isReadOnly = true}.BindProperty(component.Value).AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new PropertyField{bindingPath = "_groupId"});
            groupSettings.Add(new PropertyField{bindingPath = "_collisionFilter"});
            groupSettings.Add(new PropertyField{bindingPath = "_key"});
            groupSettings.Add(new PropertyField{bindingPath = "_autoRead"});
            groupSettings.Add(new PropertyField{bindingPath = "_cyclic"});

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