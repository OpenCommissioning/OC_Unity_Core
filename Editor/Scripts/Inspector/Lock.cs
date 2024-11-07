using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Interactions.Lock), true), CanEditMultipleObjects]
    public class Lock : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Interactions.Lock;
            if (component == null) return null;
            
            var container = new VisualElement();
            
            var groupControl = new PropertyGroup("Control");
            groupControl.AddOverrideOption(component);
            groupControl.Add(new ToggleButton("Lock").BindProperty(component.LockSignal)); 

            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new LampField("Lock", Color.green).BindProperty(component.LockSignal).AlignedField());
            groupStatus.Add(new LampField("Closed", Color.green).BindProperty(component.Closed).AlignedField());
            groupStatus.Add(new LampField("Locked", Color.green).BindProperty(component.Locked).AlignedField());
            
            var groupSettings = new PropertyGroup("References");
            groupSettings.Add(new PropertyField{bindingPath = "_doors"});
            groupSettings.Add(new PropertyField{bindingPath = "_buttons"});

            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField{bindingPath = "OnLockChanged"});
            groupEvents.Add(new PropertyField{bindingPath = "OnClosedChanged"});
            groupEvents.Add(new PropertyField{bindingPath = "OnLockedChanged"});

            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupSettings);
            container.Add(new PropertyField{bindingPath = "_link"});
            container.Add(groupEvents);
            
            return container;
        }
    }
}