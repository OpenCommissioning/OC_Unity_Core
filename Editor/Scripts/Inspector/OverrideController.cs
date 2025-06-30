using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class OverrideController : ToggleButton
    {
        private const string PROPERTY_PATH = "_override._value";
        
        public OverrideController(SerializedObject serializedObject, VisualElement target)
        {
            label = "Override";
            
            var serializedProperty = serializedObject.FindProperty(PROPERTY_PATH);
            if (serializedProperty == null)
            {
                Debug.LogError($"Serialized Property {PROPERTY_PATH} is null!");
                return;
            }

            this.BindProperty(serializedProperty);
            this.TrackPropertyValue(serializedProperty, property => target.SetEnabled(property.boolValue));
            target.SetEnabled(serializedProperty.boolValue);
        }
    }
}