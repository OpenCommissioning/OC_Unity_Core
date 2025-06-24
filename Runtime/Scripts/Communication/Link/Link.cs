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
        
        public string Path
        {
            get => _path;
            set => _path = value;
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

        public bool IsActive => _connected.Value && !_override.Value;
        
        public Property<bool> Connected => _connected;
        public Property<bool> Override => _override;
        
        public List<LinkAttribute> Attributes => _attributes;
        
        public byte Control;
        public byte Status;

        [SerializeField]
        private bool _enable = true;
        [SerializeField]
        private Property<bool> _override = new (false);
        [SerializeField]
        private Property<bool> _connected = new (false);
        
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _path;
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

        public Link()
        {
            
        }
        
        public Link(Component component, string type)
        {
            Initialize(component);
            _type = type;
        }

        public void Initialize(Component component)
        {
            _component = component;
            _name = this.GetHierarchyName();
            _path = this.GetHierarchyPath();
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
                new() { Name = Path + ".Control", Direction = ClientVariableDirection.Input },
                new() { Name = Path + ".Status", Direction = ClientVariableDirection.Output }
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
                    Logging.Logger.Log(LogType.Warning, $"{description.Name} can't be found in client!", Component);
                    result = false;
                    continue;
                }

                if (variable.Reserved)
                {
                    Logging.Logger.Log(LogType.Warning, $"{description.Name} already reserved!", Component);
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
