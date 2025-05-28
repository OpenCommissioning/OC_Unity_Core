namespace OC.Communication
{
    [System.Serializable]
    public class ConnectorDataWord : Connector
    {
        public short ControlData;
        public short StatusData;

        public ConnectorDataWord(Link link) : base(link)
        {

        }

        protected override void CreateVariableDescription()
        {
            base.CreateVariableDescription();
            _variablesDescription.Add(new ClientVariableDescription { Name = _link.Path + ".ControlData", Direction = ClientVariableDirection.Input });
            _variablesDescription.Add(new ClientVariableDescription { Name = _link.Path + ".StatusData", Direction = ClientVariableDirection.Output });
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
