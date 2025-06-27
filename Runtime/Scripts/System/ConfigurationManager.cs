using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using OC.Communication;
using UnityEngine;

namespace OC.Project
{
    public class ConfigurationManager : MonoBehaviour
    {
        public static ConfigurationManager Instance { get; private set; }

        public List<Client> Clients => _clients;
        public List<IDevice> Devices => _devices;

        private List<Client> _clients = new ();
        private List<IDevice> _devices = new ();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeInit()
        {
            if (Instance != null) return;
            
            var gameObject = new GameObject { name = "[Configuration Manager]" };
            Instance = gameObject.AddComponent<ConfigurationManager>();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneConfigurationManager.LoadFile();
        }

        private void Start()
        {
            _clients = FindObjectsOfType<Client>().ToList();
            _devices = FindObjectsOfType<MonoBehaviour>().OfType<IDevice>().ToList();
        }

        [Button]
        public void SaveConfig()
        {
            SceneConfigurationManager.SaveFile();
        }

        public void ConnectClients()
        {
            foreach (var client in _clients)
            {
                client.Connect();
            }
        }
        
        public void DisconnectClients()
        {
            foreach (var client in _clients)
            {
                client.Disconnect();
            }
        }
        
        public void ReconnectClients()
        {
            foreach (var client in _clients)
            {
                client.Disconnect();
                client.Connect();
            }
        }

        public void ResetForce()
        {
            foreach (var item in _devices.Where(item => item.Override.Value))
            {
                item.Override.Value = false;
                Logging.Logger.Log(LogType.Log, $"Link Override is DISABLED: {item.Link.ScenePath}");
            }
        }

        public void PrintForce()
        {
            foreach (var item in _devices.Where(item => item.Override.Value))
            {
                Logging.Logger.Log(LogType.Log, $"Link Override is ACTIVE: {item.Link.ScenePath}");
            }
        }
    }
}
