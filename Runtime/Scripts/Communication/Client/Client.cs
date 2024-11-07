using System;
using System.Collections.Generic;
using System.Xml.Linq;
using OC.PlayerLoop;
using OC.Project;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OC.Communication
{
    [DisallowMultipleComponent]
    public abstract class Client : MonoBehaviour, IClient, IConfigAsset, IAfterFixedUpdate, IBeforeFixedUpdate
    {
        public Component Component => this;
        public IPropertyReadOnly<bool> IsConnected => _isConnected;
        public abstract IClientBuffer Buffer { get; }
        public string RootName => _rootName;
        public bool Verbose
        {
            get => _verbose;
            set => _verbose = value;
        }
        
        public float TimeScale
        {
            get => _connector.TimeScale;
            set => _connector.TimeScale = value;
        }

        [SerializeField]
        protected Property<bool> _isConnected = new (false);
        
        [Header("Settings")]
        [SerializeField] 
        protected string _rootName = "MAIN";
        [SerializeField]
        protected bool _verbose;

        [HideInInspector]
        [SerializeField]
        private Link _link;
        [HideInInspector]
        [SerializeField]
        private ConnectorClient _connector;
        
        private List<Link> _links;

        protected void OnEnable()
        {
            BeforeFixedUpdateSystem.Register(this);
            AfterFixedUpdateSystem.Register(this);
            _isConnected.ValueChanged += OnConnectionChanged;
        }

        protected void OnDisable()
        {
            BeforeFixedUpdateSystem.Unregister(this);
            AfterFixedUpdateSystem.Unregister(this);
            _isConnected.ValueChanged -= OnConnectionChanged;
        }

        protected void Start()
        {
            _link = new Link(this, "fbSystem", $"{_rootName}.fbSystem");
            _connector = new ConnectorClient(_link)
            {
                TimeScale = Time.timeScale
            };

            _links = new List<Link> { _link };

            foreach (var component in GetComponentsInChildren<IConnectable>())
            {
                if (!component.Link.Enable) continue;
                _links.Add(component.Link);
            }
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

        private bool TryFindVariable(ClientVariableDescription discription, Object target, out ClientVariable variable)
        {
            try
            {
                var adsVariable = discription.Direction == ClientVariableDirection.Input ? 
                    Buffer.InputVariables.Find(x => x.Name == discription.Name) :
                    Buffer.OutputVariables.Find(x => x.Name == discription.Name);

                variable = adsVariable ?? throw new Exception($"{discription.Name} can't be found in client!");
                if (variable.Reserved) throw new Exception($"{discription.Name} already reserved!");
                variable.Reserved = true;
                return true;
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Warning, exception.Message, target);
                variable = null;
                return false; 
            }
        }

        public bool TryGetVariables(List<ClientVariableDescription> list, Link link, out List<ClientVariable> variables)
        {
            var valid = true;
            variables = new List<ClientVariable>();
            
            foreach (var discription in list)
            {
                if (TryFindVariable(discription, link.Component, out var variable))
                {
                    variables.Add(variable);
                }
                else
                {
                    valid = false;
                }
            }

            return valid;
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
