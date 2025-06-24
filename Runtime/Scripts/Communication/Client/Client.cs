using System.Collections.Generic;
using System.Xml.Linq;
using OC.PlayerLoop;
using OC.Project;
using UnityEngine;

namespace OC.Communication
{
    [DisallowMultipleComponent]
    public abstract class Client : MonoBehaviour, IClient, IConfigAsset, IAfterFixedUpdate, IBeforeFixedUpdate
    {
        public Component Component => this;
        public IPropertyReadOnly<bool> IsConnected => _isConnected;
        public abstract IClientBuffer Buffer { get; }
        public string RootName => _rootName;
        
        public float TimeScale
        {
            get => _link.TimeScale;
            set => _link.TimeScale = value;
        }

        [SerializeField]
        protected Property<bool> _isConnected = new (false);
        
        [Header("Settings")]
        [SerializeField] 
        protected string _rootName = "MAIN";

        [HideInInspector]
        [SerializeField]
        private LinkDataSystem _link;
        
        private List<Link> _links;

        protected void OnEnable()
        {
            BeforeFixedUpdateSystem.Register(this);
            AfterFixedUpdateSystem.Register(this);
            _isConnected.OnValueChanged += OnConnectionChanged;
        }

        protected void OnDisable()
        {
            BeforeFixedUpdateSystem.Unregister(this);
            AfterFixedUpdateSystem.Unregister(this);
            _isConnected.OnValueChanged -= OnConnectionChanged;
        }

        protected void Start()
        {
            _link.ScenePath = $"{_rootName}.fbSystem";
            _link.ClientPath = _link.ScenePath.GetClientCompatiblePath();
            _link.TimeScale = Time.timeScale;

            _links = new List<Link> { _link };

            foreach (var component in GetComponentsInChildren<ILink>())
            {
                if (!component.Link.Enable) continue;
                _links.Add(component.Link);
            }
        }

        private void Reset()
        {
            _link = new LinkDataSystem
            {
                Name = "fbSystem",
                Component = this
            };
        }
        
        public virtual void BeforeFixedUpdate()
        {
            if (!_isConnected) return;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _links.Count; i++)
            {
                _links[i].Read();
            }
        }
        
        public virtual void AfterFixedUpdate()
        {
            if (!_isConnected) return;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _links.Count; i++)
            {
                _links[i].Write();
            }
        }

        public abstract void Connect();
        public abstract void Disconnect();

        public ClientVariable GetClientVariable(ClientVariableDescription description)
        {
            return description.Direction == ClientVariableDirection.Input ? 
                Buffer.InputVariables.Find(x => x.Name == description.Path) :
                Buffer.OutputVariables.Find(x => x.Name == description.Path);
        }
        
        private void OnConnectionChanged(bool isConnected)
        {
            if (_isConnected.Value)
            {
                foreach (var link in _links)
                {
                    link.Connect(this);
                }
            }
            else
            {
                foreach (var link in _links)
                {
                    link.Disconnect();
                }
            }
        }

        public abstract XElement GetAsset();
        public abstract void SetAsset(XElement xElement);
    }
}
