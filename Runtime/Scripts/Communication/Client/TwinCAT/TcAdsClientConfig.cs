using System;

namespace OC.Communication.TwinCAT
{
    [Serializable]
    public struct TcAdsClientConfig
    {
        public string Name;
        public bool Reconnect;
        public string NetId;
        public int Port;
        public bool Verbose;

        public TcAdsClientConfig(string name, bool reconnect, string netId, int port, bool verbose = false)
        {
            Name = name;
            Reconnect = reconnect;
            NetId = netId;
            Port = port;
            Verbose = verbose;
        }

        public TcAdsClientConfig(TcAdsClient tcAdsClient)
        {
            Name = tcAdsClient.name;
            Reconnect = tcAdsClient.AutoConnect;
            NetId = tcAdsClient.NetId;
            Port = tcAdsClient.Port;
            Verbose = tcAdsClient.Verbose;
        }

        public static TcAdsClientConfig Default => new ("Client", true, "Local", 351);
    }
}