namespace OC.Communication
{
    [System.Serializable]
    public class ConnectorDataRobot : Connector
    {
        public float[] JointTarget = new float[12];
        public float[] JointStatus = new float[12];
        
        public ConnectorDataRobot(Link link) : base(link)
        {

        }

        protected override void CreateVariableDescription()
        {
            base.CreateVariableDescription();
            _variablesDescription.Add(new ClientVariableDescription { Name = _link.Path + ".JointTarget", Direction = ClientVariableDirection.Input });
            _variablesDescription.Add(new ClientVariableDescription { Name = _link.Path + ".JointStatus", Direction = ClientVariableDirection.Output });
        }

        public override void Read()
        {
            base.Read();
            _variables[2].Read(ref JointTarget);
        }

        public override void Write()
        {
            base.Write();
            _variables[3].Write(ref JointStatus);
        }
    }
}
