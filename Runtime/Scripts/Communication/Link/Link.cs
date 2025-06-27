using System;
using System.Collections.Generic;
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

        public string Name
        {
            get => _name;
            set => _name = value;
        }
        
        public string ScenePath
        {
            get => _scenePath;
            set => _scenePath = value;
        }

        public string ClientPath
        {
            get => _clientPath;
            set => _clientPath = value;
        }

        public Component Component
        {
            get => _component;
            set => _component = value;
        }
        
        public Hierarchy Parent => _parent;
        public Client Client => _client;
        public string Type 
        {
            get => _type;
            set => _type = value;
        }
        
        public Property<bool> Connected => _connected;
        
        
        public List<LinkAttribute> Attributes => _attributes;
        
        public byte Control;
        public byte Status;

        [SerializeField]
        private bool _enable = true;
        [SerializeField]
        private Property<bool> _connected = new (false);
        
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _scenePath;
        [SerializeField]
        private string _clientPath;
        [SerializeField]
        private string _type;
        [SerializeField]
        private Hierarchy _parent;
        [SerializeField]
        private List<LinkAttribute> _attributes = new ();

        private Component _component;
        private Client _client;
        
        [SerializeField]
        protected List<ClientVariableDescription> _variablesDescription = new();
        
        protected List<ClientVariable> _variables = new();

        public void Initialize(Component component)
        {
            _component = component;
            _name = this.GetHierarchyName();
            _scenePath = this.GetHierarchyPath();
            _clientPath = _scenePath.GetClientCompatiblePath();
        }

        public void Connect(Client client)
        {
            if (!_enable)
            {
                _connected.Value = false;
                return;
            }

            if (client is null)
            {
                _connected.Value = false;
                return;
            }

            _client = client;

            _variablesDescription = GetClientVariableDescriptions();
            _connected.Value = TryGetVariables(_variablesDescription, out _variables);
        }
        
        public void Disconnect()
        {
            _connected.Value = false;
        }
        
        public void Read()
        {
            if (!_connected.Value) return;
            if (!_enable) return;
            ReadClientVariables();
        }
        
        public void Write()
        {
            if (!_connected.Value) return;
            if (!_enable) return;
            WriteClientVariables();
        }

        protected virtual List<ClientVariableDescription> GetClientVariableDescriptions()
        {
            var descriptions = new List<ClientVariableDescription>
            {
                new() { Path = ClientPath + ".Control", Direction = ClientVariableDirection.Input },
                new() { Path = ClientPath + ".Status", Direction = ClientVariableDirection.Output }
            };
            return descriptions;
        }

        protected virtual void ReadClientVariables()
        {
            _variables[0].Read(ref Control);
        }

        protected virtual void WriteClientVariables()
        {
            _variables[1].Write(Status);
        }

        private bool TryGetVariables(IReadOnlyList<ClientVariableDescription> variableDescriptions, out List<ClientVariable> variables)
        {
            var result = true;
            variables = new List<ClientVariable>();
            
            foreach (var description in variableDescriptions)
            {
                var variable = _client.GetClientVariable(description);
                if (variable == null)
                {
                    Logging.Logger.Log(LogType.Warning, $"{description.Path} can't be found in client!", Component);
                    result = false;
                    continue;
                }

                if (variable.Reserved)
                {
                    Logging.Logger.Log(LogType.Warning, $"{description.Path} already reserved!", Component);
                    result = false;
                    continue;
                }
                
                variable.Reserved = true;
                variables.Add(variable);
            }
            return result;
        }
    }
}
