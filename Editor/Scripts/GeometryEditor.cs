using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class GeometryEditor : EditorWindow
    {
        private static GeometryEditor Instance { get; set; }
        public GameObject Root { get; private set; }
        public float VolumeFilter => _filterVolume;
        public float VerticesDensity => _verticesDensity;
        public bool InvertSelection => _invertSelection;

        private const string EDITOR_PREFS_ROOT = "GeometryEditor_Root";
        private const string EDITOR_FILTER_VOLUME = "GeometryEditor_FilterVolume";
        private const string EDITOR_FILTER_VERTICES = "GeometryEditor_FilterVerices";
        private const string TAG = "<b><color=#737cff>Geometry Editor</color></b>";

        [SerializeField]
        private float _filterVolume = 0.1f;
        [SerializeField]
        private int _verticesDensity = 20;
        [SerializeField]
        private bool _invertSelection;

        [MenuItem("Tools/Geometry Editor/Geometry Editor Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<GeometryEditor>();
            window.titleContent = new GUIContent("Geometry Editor");
        }

        [MenuItem("Tools/Geometry Editor/Reparenting &t", true, 400)]
        public static bool VerifyReparenting()
        {
            return Instance != null && Selection.count > 0;
        }

        [MenuItem("Tools/Geometry Editor/Reparenting &t", false, 400)]
        public static void Reparenting()
        {
            ReparentingSelectedObjects();
        }

        [MenuItem("Tools/Geometry Editor/Select Parent &t", true, 400)]
        public static bool VerifySelectParent()
        {
            return Instance != null && Selection.count > 0;
        }

        [MenuItem("Tools/Geometry Editor/Select Parent &w", false, 400)]
        public static void SelectParent()
        {
            var parent = Selection.activeGameObject.transform.parent;
            if (parent == null) return;
            Selection.activeObject = parent;
            EditorGUIUtility.PingObject(Selection.activeGameObject);
        }

        [MenuItem("Tools/Geometry Editor/Select Meshes (Size Filter) &y", true, 400)]
        public static bool VerifySelectMeshWithSize()
        {
            return Instance != null && Selection.count > 0;
        }

        [MenuItem("Tools/Geometry Editor/Select Meshes (Size Filter) &y", false, 400)]
        public static void SelectMeshWithSize()
        {
            var target = Selection.activeGameObject;
            if (target == null) return;
            var meshFilters = target.GetComponentsInChildren<MeshFilter>();

            var selectedObjects = new List<GameObject>();

            for (var i = 0; i < meshFilters.Length; i++)
            {
                var progress = i / meshFilters.Length;
                UnityEditor.EditorUtility.DisplayProgressBar("Select Meshes with Filter", "Mesh volume calculation", progress);
                var meshFilter = meshFilters[i].GetComponent<MeshFilter>();
                var volume = Math.VolumeOfMesh(meshFilter.sharedMesh);

                if (volume < Instance.VolumeFilter) continue;

                var verticesDensity = meshFilter.sharedMesh.vertexCount / (volume * 1e6);
                if (Instance.InvertSelection)
                {
                    if (verticesDensity <= Instance.VerticesDensity) selectedObjects.Add(meshFilters[i].gameObject);
                }
                else
                {
                    if (verticesDensity > Instance.VerticesDensity) selectedObjects.Add(meshFilters[i].gameObject);
                }

            }

            UnityEditor.EditorUtility.ClearProgressBar();
            if (selectedObjects.Count > 0)
            {
                Selection.objects = selectedObjects.ToArray<Object>();
            }
        }

        private void OnEnable()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            Instance = null;

            if (Root != null)
            {
                EditorPrefs.SetString(EDITOR_PREFS_ROOT, AssetDatabase.GetAssetOrScenePath(Root));
            }
            else
            {
                EditorPrefs.DeleteKey(EDITOR_PREFS_ROOT);
            }

            EditorPrefs.SetFloat(EDITOR_FILTER_VOLUME, _filterVolume);
            EditorPrefs.SetInt(EDITOR_FILTER_VERTICES, _verticesDensity);
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            var rootObjectField = new ObjectField("Root GameObject")
            {
                objectType = typeof(GameObject),
                allowSceneObjects = true
            };

            rootObjectField.RegisterCallback<ChangeEvent<Object>>((evt) => { Root = (GameObject)evt.newValue; });

            if (EditorPrefs.HasKey(EDITOR_PREFS_ROOT))
            {
                var rootGameObject = GameObject.Find(EditorPrefs.GetString(EDITOR_PREFS_ROOT));
                if (rootGameObject != null) Root = rootGameObject;
            }

            var filterVolumeField = new FloatField("Filter Volume");

            if (EditorPrefs.HasKey(EDITOR_FILTER_VOLUME))
            {
                _filterVolume = EditorPrefs.GetFloat(EDITOR_FILTER_VOLUME);
            }

            filterVolumeField.SetValueWithoutNotify(_filterVolume);
            filterVolumeField.RegisterValueChangedCallback(evt => _filterVolume = evt.newValue);

            var filterVerticesField = new IntegerField("Filter Vertices");

            if (EditorPrefs.HasKey(EDITOR_FILTER_VERTICES))
            {
                _verticesDensity = EditorPrefs.GetInt(EDITOR_FILTER_VERTICES);
            }

            filterVerticesField.SetValueWithoutNotify(_verticesDensity);
            filterVerticesField.RegisterValueChangedCallback(evt => _verticesDensity = evt.newValue);

            var invertSelectionToggle = new Toggle("Invert Selection");
            invertSelectionToggle.SetValueWithoutNotify(_invertSelection);
            invertSelectionToggle.RegisterValueChangedCallback(evt => _invertSelection = evt.newValue);

            var selectFilterVolumeButton = new Button(SelectMeshWithSize)
            {
                text = "Filter on Volume"
            };

            root.Add(rootObjectField);
            root.Add(filterVolumeField);
            root.Add(filterVerticesField);
            root.Add(invertSelectionToggle);
            root.Add(selectFilterVolumeButton);
        }

        private static void ReparentingSelectedObjects()
        {
            if (Instance.Root == null)
            {
                Log(LogType.Error, "Can't apply material! Material Palette is null!");
                return;
            }

            var selectedObjects = Selection.gameObjects;

            foreach (var selectedObject in selectedObjects)
            {
                selectedObject.transform.parent = Instance.Root.transform;
            }

            Log(LogType.Log, $"Reparenting {selectedObjects.Length} Object(s) in {AssetDatabase.GetAssetOrScenePath(Instance.Root)}");
        }

        private static void Log(LogType logType, object message)
        {
            Debug.unityLogger.Log(logType, TAG, message);
        }
    }
}
