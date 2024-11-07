using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using OC.Communication.TwinCAT;
using OC.Data;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace OC.Project
{
    public static class SceneConfiguration
    {
        private const string CONFIG_FILE_NAME = "Config";
        private const string TAG = "<color=#b78cf9>Scene Configuration Manager</color>";
        private const string ROOT = "SceneConfiguration";
        
        public static void Create()
        {
            var path = FileBrowser.SaveFilePanel("Create Scene Configuration", Application.streamingAssetsPath, GetFileName(), "json");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            
            CreateConfigurationFile(path);
        }
        
        public static void Load()
        {
            var paths = FileBrowser.OpenFilePanel("Load Scene Configuration", Application.streamingAssetsPath, "xml", false);
            if (paths == null || paths.Length == 0)
            {
                return;
            }
            
            var path = paths[0];
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            
            LoadConfiguration(path);
        }

        public static void LoadFromDefaultPath()
        {
            var path = GetDefaultFilePath();
            LoadConfiguration(path);
        }
        
        public static void SaveInDefaultPath()
        {
            var path = GetDefaultFilePath();
            CreateConfigurationFile(path);
        }

        private static void LoadConfiguration(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return;
                }
                
                using var stream = File.Open(path, FileMode.Open);
                var document = XDocument.Load(stream);
                
                var root  = document.Descendants(ROOT).FirstOrDefault();
                if (root is null)
                {
                    throw new Exception($"{TAG} Configuration isn't valid! {path}");
                }

                
                
                var configAssets = Object.FindObjectsByType<MonoBehaviour>(sortMode: FindObjectsSortMode.InstanceID).OfType<IConfigAsset>().ToList();
                foreach (var item in configAssets)
                {
                    if (TryFindElementByAttribute(root, item.Component.name, out var asset))
                    {
                        item.SetAsset(asset);
                    }
                }
            
                Logging.Logger.Log(LogType.Log, $"{TAG} Configuration loaded {path}");
                
            }
            catch (Exception exception)
            {
                Logging.Logger.LogError(exception);
            }
        }

        private static void CreateConfigurationFile(string path)
        {
            try
            {
                var config = new Configuration();
                var clients = Object.FindObjectsByType<MonoBehaviour>(sortMode: FindObjectsSortMode.InstanceID).OfType<TcAdsClient>().ToList();

                foreach (var client in clients)
                {
                    config.Clients.Add(new TcAdsClientConfig(client));
                }
                
                var directories = Object.FindObjectsByType<MonoBehaviour>(sortMode: FindObjectsSortMode.InstanceID).OfType<ProductDataDirectoryManager>().ToList();
                if (directories.Count > 0) config.Directories = directories[0].ProductDataDirectories;

                var data = JsonUtility.ToJson(config);
                File.WriteAllText(path, data);
            
                Logging.Logger.Log(LogType.Log, $"{TAG} Saved Client Configuration to {path}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static string GetFileName()
        {
            var sceneName = SceneManager.GetActiveScene().name.Replace(" ", "_").Replace(".", "_");
            return $"{sceneName}_{CONFIG_FILE_NAME}";
        }
        
        private static string GetDefaultFilePath()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            
            return $"{Application.streamingAssetsPath}/{GetFileName()}";
        }
        
        private static bool TryFindElementByAttribute(XContainer root, string value, out XElement xElement)
        {
            xElement = root.Elements().FirstOrDefault(element => element.Attribute("Name")?.Value == value);
            return xElement is not null; 
        }
        
        private static XDocument CreateEmpty()
        {
            var document = new XDocument(); 
            var root = new XElement(ROOT);
            document.Add(root);
            return document;
        }

        private class Configuration
        {
            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            // ReSharper disable once CollectionNeverQueried.Local
            public List<TcAdsClientConfig> Clients = new ();
            // ReSharper disable once NotAccessedField.Local
            public List<ProductDataDirectory> Directories = new ();
        }
    }
}
