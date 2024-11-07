namespace OC.Communication
{
    [System.Serializable]
    public class ConnectorDataLWord : Connector
    {
        public long ControlData;
        public long StatusData;

        public ConnectorDataLWord(Link link) : base(link)
        {

        }

        protected override void CreateVariableDescription()
        {
            base.CreateVariableDescription();
            _variablesDiscription.Add(new ClientVariableDescription { Name = _link.Path + ".ControlData", Direction = ClientVariableDirection.Input });
            _variablesDiscription.Add(new ClientVariableDescription { Name = _link.Path + ".StatusData", Direction = ClientVariableDirection.Output });
        }

        public override void Read()
        {
            base.Read();
            _variables[2].Read(ref ControlData);
        }

        public override void Write()
        {
            base.Write();
            _variables[3].Write(StatusData);
        }
    }
}
