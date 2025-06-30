using System;
using System.Collections.Generic;

namespace OC.Communication
{
    [Serializable]
    public class LinkDataDWord : Link
    {
        public uint ControlData;
        public uint StatusData;
        
        public LinkDataDWord(string type) : base(type){}
        
        protected override List<ClientVariableDescription> GetClientVariableDescriptions()
        {
            var descriptions = new List<ClientVariableDescription>
            {
                new() { Path = ClientPath + ".Control", Direction = ClientVariableDirection.Input },
                new() { Path = ClientPath + ".Status", Direction = ClientVariableDirection.Output },
                new() { Path = ClientPath + ".ControlData", Direction = ClientVariableDirection.Input },
                new() { Path = ClientPath + ".StatusData", Direction = ClientVariableDirection.Output }
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