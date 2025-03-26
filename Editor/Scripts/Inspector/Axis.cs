using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Components.Axis), true), CanEditMultipleObjects]
    public class Axis : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Components.Axis;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            var overrideToggle = new ToggleButton("Override").BindProperty(component.Override);
            var targetElement = new FloatField("Target") { isReadOnly = false }.BindProperty(component.Target).AlignedField();
            
            groupControl.AddOptions(overrideToggle);
            groupControl.Add(new ObjectField("Actor"){bindingPath = "_actor", objectType = typeof(Components.Actor)}.AlignedField());
            groupControl.Add(targetElement);

            var groupStatus = new PropertyGroup("Status");
            var valueElement = new FloatField("Value") { isReadOnly = true }.BindProperty(component.Value).AlignedField();
            groupStatus.Add(valueElement);

            component.Override.ValueChanged += targetElement.SetEnabled;
            targetElement.SetEnabled(component.Override.Value);
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new FloatField("Factor"){bindingPath = "_factor"}.AlignedField());
            groupSettings.Add(new EnumField("Direction"){bindingPath = "_direction"}.AlignedField());
            groupSettings.Add(new EnumField("Type"){bindingPath = "_type"}.AlignedField());
            groupSettings.Add(new EnumField("Control Mode"){bindingPath = "_controlMode"}.AlignedField());
            groupSettings.Add(new EnumField("Update Loop"){bindingPath = "_updateLoop"}.AlignedField());
            
            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupSettings);
            
            return container;
        }
    }
}
