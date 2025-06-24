using System;
using System.Collections.Generic;

namespace OC.Communication
{
    [Serializable]
    public class LinkDataLWord : Link
    {
        public long ControlData;
        public long StatusData;
        
        protected override List<ClientVariableDescription> GetClientVariableDescriptions()
        {
            var descriptions = new List<ClientVariableDescription>
            {
                new() { Name = Path + ".Control", Direction = ClientVariableDirection.Input },
                new() { Name = Path + ".Status", Direction = ClientVariableDirection.Output },
                new() { Name = Path + ".ControlData", Direction = ClientVariableDirection.Input },
                new() { Name = Path + ".StatusData", Direction = ClientVariableDirection.Output }
            };
            return descriptions;
        }

        protected override void ReadClientVariables()
        {
            _variables[0].Read(ref Control);
            _variables[2].Read(ref ControlData);
        }

        protected override void WriteClientVariables()
        {
            _variables[1].Write(Status);
            _variables[3].Write(StatusData);
        }
    }
}