using OC.VisualElements;
using OC.Interactions.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Interactions.SwitchRotary), true), CanEditMultipleObjects]
    public class SwitchRotary : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Interactions.SwitchRotary;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new IntegerField("Index"){isReadOnly = true}.BindProperty(component.Index).AlignedField());
            groupStatus.Add(new FloatField("Angle"){isReadOnly = true}.BindProperty(component.Angle).AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new PropertyField{bindingPath = "_stateCount"});
            groupSettings.Add(new PropertyField{bindingPath = "_range"});
            groupSettings.Add(new PropertyField{bindingPath = "_offset"});

            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField { bindingPath = "OnRotationChanged" });
            groupEvents.Add(new PropertyField { bindingPath = "OnIndexChanged" });

            container.Add(Factory.Create(component));
            container.Add(groupStatus);
            container.Add(groupSettings);
            container.Add(new PropertyField { bindingPath = "_link" });
            container.Add(groupEvents);
            
            return container;
        }
    }
}