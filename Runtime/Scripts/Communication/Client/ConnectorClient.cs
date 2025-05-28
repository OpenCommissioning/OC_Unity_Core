namespace OC.Communication
{
    [System.Serializable]
    public class ConnectorClient : Connector
    {
        // ReSharper disable once InconsistentNaming
        public float TimeScale;
        
        public ConnectorClient(Link link) : base(link)
        {
            
        }

        protected override void CreateVariableDescription()
        {
            base.CreateVariableDescription();
            _variablesDescription.Add(new ClientVariableDescription { Name = _link.Path + ".TimeScaling", Direction = ClientVariableDirection.Output });
        }
        
        public override void Read()
        {
            base.Read();
            Status.SetBit(7, Control.GetBit(7));
        }

        public override void Write()
        {
            base.Write();
            _variables[2].Write(ref TimeScale);
        }
    }
}