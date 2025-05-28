using OC.Interactions.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace OC.Editor.Inspector
{
    [CustomEditor(typeof(Interactions.PanelSampler), false), CanEditMultipleObjects]
    public class PanelSampler : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var component = target as Interactions.PanelSampler;
            if (component == null) return null;
            
            var container = new VisualElement();

            var groupSettings = new PropertyGroup("Settings");
            groupSettings.Add(new PropertyField{bindingPath = "_type"});
            groupSettings.Add(new PropertyField{bindingPath = "_components"});

            var panel = new VisualElement();
            foreach (var item in component.Components)
            {
                var modul = Factory.Create(item);
                if (modul != null) panel.Add(modul);
            }
            
            container.Add(panel);
            container.Add(groupSettings);
            container.Add(new PropertyField{bindingPath = "_link"});
            
            return container;
        }
    }
}