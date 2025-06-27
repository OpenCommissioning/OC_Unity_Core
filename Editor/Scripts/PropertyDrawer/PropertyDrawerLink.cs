using OC.Communication;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    [CustomPropertyDrawer(typeof(Link), true)]
    public class PropertyDrawerLink : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new PropertyGroup("Communication");
            container.AddHeaderElement(new LampField{bindingPath = "_isConnected._value", LampShape = LampField.InspectorLampShape.Round});
            container.Add(new Toggle("Is Enabled"){bindingPath = "_enable"}.AlignedField());
            container.Add(new TextField("Path"){bindingPath = "_path", isReadOnly = true}.AlignedField());
            container.Add(new TextField("Type"){bindingPath = "_type"}.AlignedField());
            container.Add(new ObjectField("Parent"){bindingPath = "_parent", objectType = typeof(Hierarchy)}.AlignedField());
            container.Add(new PropertyField{bindingPath = "_attributes"});
            return container;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}
