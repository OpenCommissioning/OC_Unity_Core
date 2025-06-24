using System;

namespace OC.Communication
{
    [Serializable]
    public struct ClientVariableDescription
    {
        public string Path;
        public ClientVariableDirection Direction;
    }
}