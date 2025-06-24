using System;
using System.Collections.Generic;

namespace OC.Communication
{
    [Serializable]
    public class LinkDataRobot : Link
    {
        public float[] JointTarget = new float[12];
        public float[] JointStatus = new float[12];
        
        protected override List<ClientVariableDescription> GetClientVariableDescriptions()
        {
            var descriptions = new List<ClientVariableDescription>
            {
                new() { Name = Path + ".Control", Direction = ClientVariableDirection.Input },
                new() { Name = Path + ".Status", Direction = ClientVariableDirection.Output },
                new() { Name = Path + ".JointTarget", Direction = ClientVariableDirection.Input },
                new() { Name = Path + ".JointStatus", Direction = ClientVariableDirection.Output }
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