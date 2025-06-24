using System;

namespace OC.Communication
{
    [Serializable]
    public struct ClientVariableDescription
    {
        public string Name;
        public ClientVariableDirection Direction;
    }
}