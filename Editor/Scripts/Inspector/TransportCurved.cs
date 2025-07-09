using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Components.TransportCurved), false), CanEditMultipleObjects]
    public class TransportCurved : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Components.TransportCurved;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            groupControl.Add(new FloatField("Target"){bindingPath = "_target._value"}.AlignedField());

            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new FloatField("Value"){isReadOnly = true, bindingPath = "_value._value"}.AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new PropertyField{bindingPath = "_gizmos"});
            groupSettings.Add(new PropertyField{bindingPath = "_actor"});
            groupSettings.Add(new PropertyField{bindingPath = "_angle"});
            groupSettings.Add(new PropertyField{bindingPath = "_radius"});
            groupSettings.Add(new PropertyField{bindingPath = "_smoothness"});
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