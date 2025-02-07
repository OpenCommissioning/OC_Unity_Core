using OC.Project;
using UnityEditor;
using UnityEngine;

namespace OC.Editor
{
    public static class SceneMenuItems 
    {
        [MenuItem("Open Commissioning/Scene/Create Snapshot", priority = 10)]
        private static void CreateSnapshot()
        {
            SnapshotManager.Create();
        }
        
        [MenuItem("Open Commissioning/Scene/Create Snapshot", true, priority = 10)]
        private static bool ValidateCreateSnapshot()
        {
            return Application.isPlaying;
        }

        [MenuItem("Open Commissioning/Scene/Load Snapshot", priority = 11)]
        public static void LoadSnapshot()
        {
            SnapshotManager.Load();
        }
        
        [MenuItem("Open Commissioning/Scene/Load Snapshot", true, priority = 11)]
        private static bool ValidateLoadSnapshot()
        {
            return Application.isPlaying;
        }
        
        [MenuItem("Open Commissioning/Scene/Create Configuration", priority = 20)]
        private static void CreateConfiguration()
        {
            SceneConfigurationManager.Save();
        }
        
        [MenuItem("Open Commissioning/Scene/Load Configuration", priority = 21)]
        private static void LoadConfiguration()
        {
            SceneConfigurationManager.Load();
        }
        
        [MenuItem("Open Commissioning/Settings/Apply Default Layers", priority = 100)]
        private static void ApplyDefaultLayers()
        {
            LayerManager.ApplyDefaultLayers();
        }

        [MenuItem("Open Commissioning/Utility/Screenshot")]
        public static void CreateScreenshot()
        {
            EditorUtility.CreateScreenshot();
        }
    }
}
