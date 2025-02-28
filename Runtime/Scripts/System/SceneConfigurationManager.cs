using System;
using UnityEngine;
using System.IO;
using OC.Data;
using UnityEngine.SceneManagement;

namespace OC.Project
{
    public static class SceneConfigurationManager
    {
        private const string CONFIG_FILE_NAME = "Config";
        private const string TAG = "<color=#b78cf9>Scene Configuration Manager</color>";
        private const string ROOT = "SceneConfiguration";
        
        public static void Save()
        {
            var path = FileBrowser.SaveFilePanel("Create Scene Configuration", Application.streamingAssetsPath, GetFileName(), "json");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                SaveFile(path);
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, $"{TAG} {exception}");
            }
        }
        
        public static void Load()
        {
            var paths = FileBrowser.OpenFilePanel("Load Scene Configuration", Application.streamingAssetsPath, "json", false);
            if (paths == null || paths.Length == 0)
            {
                return;
            }
            
            var path = paths[0];
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                LoadFile(path);
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, $"{TAG} {exception}");
            }
        }

        private static void LoadFile(string path)
        {
            path = Path.ChangeExtension(path, "json");
            new SceneConfiguration().Load(path).ApplyToActiveScene();
            Logging.Logger.Log(LogType.Log, $"{TAG} Configuration loaded {path}");
        }

        private static void SaveFile(string path)
        {
            path = Path.ChangeExtension(path, "json");
            new SceneConfiguration().Create(GetActiveSceneName()).Save(path);
            Logging.Logger.Log(LogType.Log, $"{TAG} Configuration saved {path}");
        }

        public static void LoadFile()
        {
            try
            {
                LoadFile(GetDefaultFilePath());
            }
            catch (Exception exception)
            {
                //ignore
            }
        }

        public static void SaveFile()
        {
            try
            {
                SaveFile(GetDefaultFilePath());
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, $"{TAG} {exception}");
            }
        }

        private static string GetFileName() => $"{GetActiveSceneName()}_{CONFIG_FILE_NAME}";

        private static string GetActiveSceneName()
        {
            return SceneManager.GetActiveScene().name.Replace(" ", "_").Replace(".", "_");
        }
        
        private static string GetDefaultFilePath()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            
            return $"{Application.streamingAssetsPath}/{GetFileName()}";
        }
    }
}
