using UnityEngine;
using UnityEngine.UIElements;

namespace OC.VisualElements
{
#if UNITY_6000_3_OR_NEWER
    [UxmlElement]
    public partial class StackHorizontal : VisualElement
    {
#else
    public class StackHorizontal : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<StackHorizontal, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }
#endif
         
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