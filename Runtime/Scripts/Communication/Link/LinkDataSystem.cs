using System;
using System.Collections.Generic;

namespace OC.Communication
{
    [Serializable]
    public class LinkDataSystem : Link
    {
        public float TimeScale;
        
        protected override List<ClientVariableDescription> GetClientVariableDescriptions()
        {
            var descriptions = new List<ClientVariableDescription>
            {
                new() { Path = ClientPath + ".Control", Direction = ClientVariableDirection.Input },
                new() { Path = ClientPath + ".Status", Direction = ClientVariableDirection.Output },
                new() { Path = ClientPath + ".TimeScaling", Direction = ClientVariableDirection.Output }
            };
            return descriptions;
        }

        protected override void ReadClientVariables()
        {
            _variables[0].Read(ref Control);
            Status.SetBit(7, Control.GetBit(7));
        }

        protected override void WriteClientVariables()
        {
            _variables[1].Write(Status);
            _variables[2].Write(ref TimeScale);
        }
    }
}