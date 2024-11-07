using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Interactions.UIElements
{
    public class IndustrialPanel : VisualElement
    {
        private const string STYLE_SHEET = "StyleSheet/industrial-panel";
        private const string USS_CONTAINER = "group-container"; 
        private const string USS_LABEL = "group-label";
        private const string USS_CONTENT = "group-content";

        private readonly VisualElement _content;

        public IndustrialPanel(string name)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(STYLE_SHEET));
            AddToClassList(USS_CONTAINER);
            
            var label = new Label(name.ToUpper());
            label.AddToClassList(USS_LABEL);
            hierarchy.Add(label);
            
            _content = new VisualElement();
            _content.AddToClassList(USS_CONTENT);
            hierarchy.Add(_content);
        }
        
        public new void Add(VisualElement element)
        {
            _content.Add(element);
        }
    }
}