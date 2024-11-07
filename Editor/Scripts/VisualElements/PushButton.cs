using OC.Interactions.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class PushButton : BaseField<bool>
    {
        public new class UxmlFactory : UxmlFactory<PushButton, UxmlTraits> { }
        public new class UxmlTraits : BaseFieldTraits<bool, UxmlBoolAttributeDescription>
        {
            public UxmlTraits()
            {
                focusable.defaultValue = false;
            }
        }

        private const string USS = "StyleSheet/oc-inspector";
        private const string USS_CLASS_NAME = "button";
        private const string LABEL_USS_CLASS_NAME = USS_CLASS_NAME + "__label";
        private const string CHECKED_USS_CLASS_NAME = USS_CLASS_NAME + "-checked";
        private const string UNITY_BUTTON_USS_CLASS_NAME = "unity-button";
        private const string UNITY_BASE_FIELD_INPUT_USS_CLASS_NAME = "unity-base-field__input";
        
        private readonly MouseEvents _mouseEvents;

        public PushButton() : this(null){}
        
        public PushButton(string label) : base(label, null)
        {
            this.label = label;
            focusable = false;
            
            this.Q(className:UNITY_BASE_FIELD_INPUT_USS_CLASS_NAME).style.display = DisplayStyle.None;
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            AddToClassList(UNITY_BUTTON_USS_CLASS_NAME);
            AddToClassList(USS_CLASS_NAME);
            labelElement.AddToClassList(LABEL_USS_CLASS_NAME);
            
            _mouseEvents = new MouseEvents()
            {
                target = this
            };
            
            _mouseEvents.Up += () => value = false;
            _mouseEvents.Down += () => value = true;
        }
        
        public override void SetValueWithoutNotify(bool newValue)
        {
            EnableInClassList(CHECKED_USS_CLASS_NAME, newValue);
            base.SetValueWithoutNotify(newValue);
        }
    }
}