using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace OC.Editor
{
    public class MaterialUtils : EditorWindow
    {
        private const string PREFS_MATERIAL_LAST = "PREFS_MaterialUtils_MATERIAL_LAST";
        
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

            _materialField.SetValueWithoutNotify(LoadMaterialFromPrefs());
        }

        private void OnDisable()
        {
            SafeMaterialPrefs(_material);
        }

        private Material LoadMaterialFromPrefs()
        {
            var id = PlayerPrefs.GetString(PREFS_MATERIAL_LAST);
            return string.IsNullOrEmpty(id) ? null : AssetDatabase.LoadAssetAtPath<Material>(id);
        }

        private void SafeMaterialPrefs(Material material)
        {
            if (material == null) return;
            var path = AssetDatabase.GetAssetPath(material);
            if (string.IsNullOrEmpty(path)) return;
            
            PlayerPrefs.SetString(PREFS_MATERIAL_LAST, path);
            PlayerPrefs.Save();
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
            
            var renderers = _target.GetComponentsInChildren<Renderer>(includeInactive: false);
            if (renderers == null || renderers.Length == 0) return;

            Undo.SetCurrentGroupName("Assign Material To Children");
            var group = Undo.GetCurrentGroup();
            
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
                        counter++;
                        break;
                    case > 1:
                    {
                        for (var i = 0; i < sharedMaterials.Length; i++)
                        {
                            sharedMaterials[i] = _material;
                        }

                        renderer.sharedMaterials = sharedMaterials;
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
