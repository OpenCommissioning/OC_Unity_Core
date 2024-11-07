using UnityEngine;

namespace OC.Communication
{
    public interface IClient
    {
        public Component Component { get; }
        public IPropertyReadOnly<bool> IsConnected { get; }
        public IClientBuffer Buffer { get; }
        public void Connect();
        public void Disconnect();
    }
}