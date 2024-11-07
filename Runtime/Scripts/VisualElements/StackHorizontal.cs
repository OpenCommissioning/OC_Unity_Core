using UnityEngine;
using UnityEngine.UIElements;

namespace OC.VisualElements
{
    public class StackHorizontal : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<StackHorizontal, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
         
        private const string USS = "StyleSheet/oc-default";
        private const string USS_CLASS_NAME = "stack-horizontal";
        
        public StackHorizontal()
        {
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            AddToClassList(USS_CLASS_NAME);
        }
        
        public new void Add(VisualElement visualElement)
        {
            base.Add(visualElement);
            visualElement.style.flexGrow = 1;
        }
    }
}