using OC.VisualElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Components.Cylinder), true), CanEditMultipleObjects]
    public class Cylinder : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Components.Cylinder;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupControl = new PropertyGroup("Control");
            var hStack = new StackHorizontal();
            hStack.Add(new PushButton("Minus").BindProperty(component.Minus));
            hStack.Add(new PushButton("Plus").BindProperty(component.Plus));
            groupControl.Add(hStack);
            groupControl.AddForceOption(component, new IProperty[]{component.Minus, component.Plus});

            var groupStatus = new PropertyGroup("Status");
            groupStatus.Add(new ProgressBar("Progress"){bindingPath = "_progress._value", ShowLimits = true});
            groupStatus.Add(new FloatField("Value"){isReadOnly = true}.BindProperty(component.Value).AlignedField());
            
            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new Vector2Field("Limits"){bindingPath = "_limits._value"}.AlignedField());
            groupSettings.Add(new EnumField("Type"){bindingPath = "_type._value"}.AlignedField());
            groupSettings.Add(new FloatField("Time to Min"){bindingPath = "_timeToMin._value"}.AlignedField());
            groupSettings.Add(new FloatField("Time to Max"){bindingPath = "_timeToMax._value"}.AlignedField());
            groupSettings.Add(new PropertyField{bindingPath = "_profile"});

            var groupEvents = new PropertyGroup("Events");
            groupEvents.Add(new PropertyField{bindingPath = "OnActiveChanged"});
            groupEvents.Add(new PropertyField{bindingPath = "OnLimitMinEvent"});
            groupEvents.Add(new PropertyField{bindingPath = "OnLimitMaxEvent"});
            
            container.Add(groupControl);
            container.Add(groupStatus);
            container.Add(groupSettings);
            container.Add(new PropertyField{bindingPath = "_link"});
            container.Add(groupEvents);
            
            return container;
        }
    }
}
