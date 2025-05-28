using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Interactions.Door), false), CanEditMultipleObjects]
    public class Door : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Interactions.Door;
            if (component == null) return null;
            
            var container = new VisualElement();
            
            var groupControl = new PropertyGroup("Control");
            var openButton = new UnityEngine.UIElements.Button{text = "Open"};
            var closeButton = new UnityEngine.UIElements.Button{text = "Close"};
            openButton.clicked += component.Open;
            closeButton.clicked += component.Close;
            groupControl.Add(openButton);
            groupControl.Add(closeButton);

            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Closed", Color.green).BindProperty(component.Closed).AlignedField());
            groupStatus.Add(new LampField("Lock", Color.green).BindProperty(component.Lock).AlignedField());
            groupStatus.Add(new LampField("Locked", Color.green).BindProperty(component.Locked).AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new PropertyField{bindingPath = "_target"});
            groupSettings.Add(new PropertyField{bindingPath = "_duration"});

            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField{bindingPath = "OnOpenEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnCloseEvent"});

            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupSettings);
            container.Add(groupEvents);
            
            return container;
        }
    }
}