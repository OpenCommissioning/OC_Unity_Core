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
        
        
        [MenuItem("Open Commissioning/Material Utils")]
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
            
            rootVisualElement.Add(new Button(ApplyMaterial)
            {
                text = "Set for all"
            });
            
            _objectField.RegisterCallback<ChangeEvent<Object>>((evt) =>
            {
                _target = (GameObject)evt.newValue;
            });
            
            _materialField.RegisterCallback<ChangeEvent<Object>>((evt) =>
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

        private void ApplyMaterial()
        {
            if (_target == null) return;
            if (_material == null) return;
            var counter = 0;
            
            var renderes = _target.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderes)
            {
                Material[] materials = new Material[renderer.materials.Length];
                if (renderer.materials.Length > 1)
                {
                    for (var i = 0; i < materials.Length; i++)
                    {
                        materials[i] = _material;
                    }

                    renderer.materials = materials;
                    counter++;
                }
                
                renderer.material = _material;
            }
            
            Debug.Log($"Materials Utils: {counter} materials are changed");
        }
    }
}
