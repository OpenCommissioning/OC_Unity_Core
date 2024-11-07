using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class RenameObjectsEditor : EditorWindow
    {
        private TextField _textField;
        private IntegerField _integerField;

        [MenuItem("Tools/Rename Selected", false, -1)]
        private static void ShowRenameEditor()
        {
            EditorWindow window = GetWindow<RenameObjectsEditor>();
            window.titleContent = new GUIContent("Rename selected gameObjects");
        }

        public void CreateGUI()
        {
            _textField = new TextField("Group Name");
            _textField.SetValueWithoutNotify("Module");

            _integerField = new IntegerField("Start Index");
            _integerField.SetValueWithoutNotify(1);

            rootVisualElement.Add(_textField);
            rootVisualElement.Add(_integerField);
            rootVisualElement.Add(new Button(Apply)
            {
                text = "Apply"
            });
        }

        private void Apply()
        {
            var selectedObjects = Selection.gameObjects.ToArray<Object>();
            Undo.RegisterCompleteObjectUndo(selectedObjects, "Rename GameObject Group");
            for (var i = 0; i < selectedObjects.Length; i++)
            {
                selectedObjects[i].name = $"{_textField.text}{_integerField.value + i}";
            }

            Close();
        }
    }
}
