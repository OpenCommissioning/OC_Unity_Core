using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace OC.Editor
{
    public class MaterialUtils : EditorWindow
    {
        private ObjectField _objectField;
        private ObjectField _materialField;
        private GameObject _target;
        private Material _material;
        
        
        [MenuItem("Open Commissioning/Tools/Material Utils")]
        public static void ShowMaterialManager()
        {
            var window = GetWindow<MaterialUtils>();
            window.titleContent = new GUIContent("Material Utils");
            window.Show();
        }

        private void CreateGUI()
        {
            _objectField = new ObjectField("GameObject")
            {
                objectType = typeof(GameObject)
            };

            _materialField = new ObjectField("Material")
            {
                objectType = typeof(Material)
            };
            
            
            rootVisualElement.Add(_objectField); 
            rootVisualElement.Add(_materialField); 
            
            rootVisualElement.Add(new Button(OnButtonClick)
            {
                text = "Set for all"
            });
            
            _objectField.RegisterCallback<ChangeEvent<Object>>(evt =>
            {
                _target = (GameObject)evt.newValue;
            });
            
            _materialField.RegisterCallback<ChangeEvent<Object>>(evt =>
            {
                _material = (Material)evt.newValue;
            });

            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            if (Selection.activeGameObject != null)
            {
                _target = Selection.activeGameObject;
                _objectField.SetValueWithoutNotify(_target);
            }
        }

        private void OnButtonClick() => ApplyMaterial(_target, _material);

        private void ApplyMaterial(GameObject root, Material material)
        {
            if (root == null) return;
            if (material == null) return;
            var counter = 0;

            Undo.SetCurrentGroupName("Assign Material To Children");
            var group = Undo.GetCurrentGroup();
            Undo.RegisterFullObjectHierarchyUndo(root, "Assign Material To Children");
            
            var renderers = _target.GetComponentsInChildren<Renderer>(includeInactive: false);
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;

                Undo.RecordObject(renderer, "Assign Material To Children");
                
                var sharedMaterials = new Material[renderer.sharedMaterials.Length];

                switch (sharedMaterials.Length)
                {
                    case 0:
                        continue;
                    case 1:
                        renderer.sharedMaterial = _material;
                        UnityEditor.EditorUtility.SetDirty(renderer);
                        counter++;
                        break;
                    case > 1:
                    {
                        for (var i = 0; i < sharedMaterials.Length; i++)
                        {
                            sharedMaterials[i] = _material;
                        }

                        renderer.sharedMaterials = sharedMaterials;
                        UnityEditor.EditorUtility.SetDirty(renderer);
                        counter++;
                        break;
                    }
                }
            }
            
            Undo.CollapseUndoOperations(group);
            Debug.Log($"Material Utils: {counter} materials are changed");
        }
    }
}
