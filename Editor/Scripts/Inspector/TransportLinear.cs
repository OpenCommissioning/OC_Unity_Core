using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Components.TransportLinear), true), CanEditMultipleObjects]
    public class TransportLinear : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Components.TransportLinear;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            groupControl.Add(new FloatField("Target").BindProperty(component.Target).AlignedField());

            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new FloatField("Value"){ isReadOnly = true }.BindProperty(component.Value).AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new PropertyField{bindingPath = "_gizmos"});
            groupSettings.Add(new PropertyField{bindingPath = "_actor"});
            groupSettings.Add(new PropertyField{bindingPath = "_length"});
            groupSettings.Add(new PropertyField{bindingPath = "_width"});
            groupSettings.Add(new PropertyField{bindingPath = "_height"});
            groupSettings.Add(new PropertyField{bindingPath = "_factor"});
            groupSettings.Add(new PropertyField{bindingPath = "_isDynamic"});
            groupSettings.Add(new PropertyField{bindingPath = "_isGuiding"});
            
            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupSettings);
            
            return container;
        }
    }
}