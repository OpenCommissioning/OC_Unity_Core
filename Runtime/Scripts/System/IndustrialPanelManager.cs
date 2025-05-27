using System.Collections.Generic;
using OC.Communication;
using OC.Interactions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OC.Project
{
    public class IndustrialPanelManager : MonoBehaviour
    {
        public static IndustrialPanelManager Instance { get; private set; }

        public List<IIndustrialPanel> IndustrialPanels => _industrialPanels;

        private List<IIndustrialPanel> _industrialPanels = new ();
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeInit()
        {
            if (Instance != null) return;
            var gameObject = new GameObject { name = "[Industrial Panel Manager]" };
            Instance = gameObject.AddComponent<IndustrialPanelManager>();
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _industrialPanels = new List<IIndustrialPanel>();
            var clients = new List<Client>();
            
            var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                if (rootGameObject.TryGetComponent<Client>(out var client))
                {
                    clients.Add(client);
                }
            }

            foreach (var client in clients)
            {
                var visualElements = client.GetComponentsInChildren<IIndustrialPanel>();
                _industrialPanels.AddRange(visualElements);
            }
        }
    }
}