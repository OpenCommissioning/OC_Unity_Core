using System.Collections.Generic;
using System.IO;
using System.Linq;
using OC.Communication.TwinCAT;
using OC.Data;
using UnityEngine;

namespace OC.Project
{
    public class SceneConfiguration
    {
        public string SceneName; 
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once CollectionNeverQueried.Local
        public List<TcAdsClientConfig> Clients = new ();
        // ReSharper disable once NotAccessedField.Local
        public List<ProductDataDirectory> Directories = new ();

        public SceneConfiguration Create(string sceneName)
        {
            SceneName = sceneName;
            Clients.Clear();
            var clients = Object.FindObjectsByType<MonoBehaviour>(sortMode: FindObjectsSortMode.InstanceID).OfType<TcAdsClient>().ToList();
            foreach (var client in clients)
            {
                Clients.Add(client.Config);
            }
            
            var directories = Object.FindObjectsByType<MonoBehaviour>(sortMode: FindObjectsSortMode.InstanceID).OfType<ProductDataDirectoryManager>().ToList();
            if (directories.Count > 0) Directories = directories[0].ProductDataDirectories;
            return this;
        }

        public void Save(string path)
        {
            var data = JsonUtility.ToJson(this, true);
            File.WriteAllText(path, data);
        }

        public SceneConfiguration Load(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileLoadException($"Path: {path} isn't valid!");
            }
            
            using var stream = File.OpenRead(path);
            var config = JsonUtility.FromJson<SceneConfiguration>(new StreamReader(stream).ReadToEnd());
            Clients = config.Clients;
            Directories = config.Directories;
            return this;
        }

        public void ApplyToActiveScene()
        {
            var clients = Object.FindObjectsByType<TcAdsClient>(sortMode: FindObjectsSortMode.InstanceID).ToList();
            var productDataDirectoryManagers = Object.FindObjectsByType<ProductDataDirectoryManager>(sortMode: FindObjectsSortMode.InstanceID).ToList();
            
            foreach (var client in clients)
            {
                foreach (var tcAdsClientConfig in Clients)
                {
                    if (string.Equals(client.Config.Name, tcAdsClientConfig.Name))
                    {
                        client.Config = tcAdsClientConfig;
                    }
                }
            }

            foreach (var dataDirectoryManager in productDataDirectoryManagers)
            {
                dataDirectoryManager.ProductDataDirectories = Directories;
            }
        }
    }
}