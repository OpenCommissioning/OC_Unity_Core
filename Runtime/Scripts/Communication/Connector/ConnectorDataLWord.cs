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
            _variablesDescription.Add(new ClientVariableDescription { Path = _link.CompatiblePath + ".ControlData", Direction = ClientVariableDirection.Input });
            _variablesDescription.Add(new ClientVariableDescription { Path = _link.CompatiblePath + ".StatusData", Direction = ClientVariableDirection.Output });
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
