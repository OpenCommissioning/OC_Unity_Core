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
