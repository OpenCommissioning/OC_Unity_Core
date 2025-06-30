using System;
using System.Collections.Generic;

namespace OC.Communication
{
    [Serializable]
    public class LinkDataRobot : Link
    {
        public float[] JointTarget = new float[12];
        public float[] JointStatus = new float[12];
        
        public LinkDataRobot(string type) : base(type){}
        
        protected override List<ClientVariableDescription> GetClientVariableDescriptions()
        {
            var descriptions = new List<ClientVariableDescription>
            {
                new() { Path = ClientPath + ".Control", Direction = ClientVariableDirection.Input },
                new() { Path = ClientPath + ".Status", Direction = ClientVariableDirection.Output },
                new() { Path = ClientPath + ".JointTarget", Direction = ClientVariableDirection.Input },
                new() { Path = ClientPath + ".JointStatus", Direction = ClientVariableDirection.Output }
            };
            return descriptions;
        }

        protected override void ReadClientVariables()
        {
            _variables[0].Read(ref Control);
            _variables[2].Read(ref JointTarget);
        }

        protected override void WriteClientVariables()
        {
            _variables[1].Write(Status);
            _variables[3].Write(ref JointStatus);
        }
    }
}