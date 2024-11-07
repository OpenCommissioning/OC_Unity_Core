namespace OC.Communication
{
    [System.Serializable]
    public class ConnectorDataDWord : Connector
    {
        public uint ControlData;
        public uint StatusData;

        public ConnectorDataDWord(Link link) : base(link)
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
