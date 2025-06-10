using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OC.Communication
{
    [Serializable]
    public class Link
    {
        public bool Enable
        {
            get => _enable;
            set => _enable = value;
        }
        public string Name => _name;
        public string Path => _path;
        
        /// <summary>
        /// Gets the compatible or formatted version of the original path, where 
        /// each segment contains only valid characters or is properly escaped.
        /// </summary>
        public string CompatiblePath => _compatiblePath;
        public Hierarchy Parent => _parent;
        public Client Client => _client;
        public string Type 
        {
            get => _type;
            set => _type = value;
        }

        public bool IsActive => _connected.Value && !_override.Value;
        
        public Property<bool> Connected => _connected;
        public Property<bool> Override => _override;
        public Component Component => _component;
        public List<LinkAttribute> Attributes => _attributes;

        [SerializeField]
        private bool _enable = true;
        [SerializeField]
        private Property<bool> _override = new (false);
        [SerializeField]
        private Property<bool> _connected = new (false);
        
        [SerializeField]
        private string _type;
        [SerializeField]
        private Hierarchy _parent;
        [SerializeField]
        private List<LinkAttribute> _attributes = new ();
        
        [HideInInspector, SerializeField]
        private string _name;
        [HideInInspector, SerializeField]
        private string _path;
        [HideInInspector, SerializeField]
        private string _compatiblePath;

        private Component _component;
        private Client _client;
        private List<Connector> _connectors;

        public Link(Component component, string type)
        {
            Initialize(component);
            _type = type;
        }

        public Link(Component component, string name, string path)
        {
            _component = component;
            _name = name;
            _path = path;
            _connectors = new List<Connector>();
        }
        
        public Link(Component component)
        {
            Initialize(component);
        }

        public void Initialize(Component component)
        {
            _component = component;
            _name = this.GetHierarchyName();
            _path = this.GetHierarchyPath();
            _compatiblePath = _path.GetCompatiblePath();
            _client = this.GetClient();
            _connectors = new List<Connector>();
        }

        public Link Reset(Component component)
        {
            Initialize(component);
            return this;
        }

        public void Connect(Client client)
        {
            if (client is null) return;

            _client = client;
            
            if (!_enable)
            {
                _connected.Value = false;
                return;
            }

            foreach (var connector in _connectors)
            {
                connector.Connect(client);
            }
            
            _connected.Value = _connectors.All(connector => connector.IsConnected);
        }
        
        public void Disconnect()
        {
            _connected.Value = false;
        }
        
        public void Read()
        {
            if (!_connected.Value) return;
            if (!_enable) return;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _connectors.Count; i++)
            {
                _connectors[i].Read();
            }
        }
        
        public void Write()
        {
            if (!_connected.Value) return;
            if (!_enable) return;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _connectors.Count; i++)
            {
                _connectors[i].Write();
            }
        }

        public void Add(Connector connector)
        {
            _connectors.Add(connector);
        }
    }
}
