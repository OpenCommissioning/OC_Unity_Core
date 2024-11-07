using System.Collections.Generic;

namespace OC.Communication
{
    [System.Serializable]
    public class Connector
    {
        public bool IsConnected { get; private set; }

        public byte Control;
        public byte Status;

        protected readonly Link _link;
        protected List<ClientVariable> _variables = new();
        protected List<ClientVariableDescription> _variablesDiscription = new();

        public Connector(Link link)
        {
            _link = link;
            _link.Add(this);
        }

        public void Connect(Client client)
        {
            IsConnected = false;
            _variablesDiscription = new List<ClientVariableDescription>();
            CreateVariableDescription();
            
            _variables = new List<ClientVariable>();
            IsConnected = client.TryGetVariables(_variablesDiscription, _link, out _variables);
        }

        protected virtual void CreateVariableDescription()
        {
            _variablesDiscription.Add(new ClientVariableDescription { Name = _link.Path + ".Control", Direction = ClientVariableDirection.Input });
            _variablesDiscription.Add(new ClientVariableDescription { Name = _link.Path + ".Status", Direction = ClientVariableDirection.Output });
        }

        public virtual void Read()
        {
            _variables[0].Read(ref Control);
        }

        public virtual void Write()
        {
            _variables[1].Write(Status);
        }
    }
}
