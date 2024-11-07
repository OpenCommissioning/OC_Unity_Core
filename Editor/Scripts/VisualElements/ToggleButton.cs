using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class ToggleButton : Toggle
    {
        public new class UxmlFactory : UxmlFactory<ToggleButton, UxmlTraits> { }
        public new class UxmlTraits : Toggle.UxmlTraits
        {
            public UxmlTraits()
            {
                focusable.defaultValue = false;
            }
        }

        private const string USS = "StyleSheet/oc-inspector";
        private const string USS_CLASS_NAME = "button";
        private const string LABEL_USS_CLASS_NAME = USS_CLASS_NAME + "__label";
        private const string UNITY_BUTTON_USS_CLASS_NAME = "unity-button";
        private const string UNITY_BASE_FIELD_INPUT_USS_CLASS_NAME = "unity-base-field__input";

        private IProperty<bool> _property;

        public ToggleButton() : this("Toggle Button"){}

        public ToggleButton(string label) : base(null)
        {
            this.label = label;
            focusable = false;

            this.Q(className:UNITY_BASE_FIELD_INPUT_USS_CLASS_NAME).style.display = DisplayStyle.None;
            
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            RemoveFromClassList(ussClassName);
            AddToClassList(UNITY_BUTTON_USS_CLASS_NAME);
            AddToClassList(USS_CLASS_NAME);
            labelElement.AddToClassList(LABEL_USS_CLASS_NAME);
        }
    }
}