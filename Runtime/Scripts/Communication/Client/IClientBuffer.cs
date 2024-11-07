using System.Collections.Generic;

namespace OC.Communication
{
    public interface IClientBuffer
    {
        public bool IsConnected { get; }
        public byte[] InputBytes { get; }
        public byte[] OutputBytes { get; }
        public List<ClientVariable> InputVariables { get; }
        public List<ClientVariable> OutputVariables { get; }
        public void Connect(string netId, int port);
        public void Disconnect();
        public void Read();
        public void Write();
    }
}