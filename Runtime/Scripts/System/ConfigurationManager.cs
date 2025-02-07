using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using OC.Communication;
using OC.Components;
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
        private List<IControlOverridable> _forceComponents = new ();

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
            _forceComponents = FindObjectsOfType<MonoBehaviour>().OfType<IControlOverridable>().ToList();
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
        
        public void DisconectClients()
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
            foreach (var item in _forceComponents.Where(item => item.Override.Value))
            {
                item.Override.Value = false;
                Logging.Logger.Log(LogType.Log, $"FORCE mode disabled: {item.Link.Path}");
            }
        }

        public void PrintForce()
        {
            var items = GetForcedComponents();
            if (items == null)
            {
                Logging.Logger.Log(LogType.Log, $"There are no forced objects");
                return;
            }
            foreach (var item in items)
            {
                Logging.Logger.Log(LogType.Log, $"FORCED: {item.Link.Path}");
            }
        }

        private List<IControlOverridable> GetForcedComponents()
        {
            return _forceComponents.Where(item => item.Override.Value).ToList();
        }
    }
}
