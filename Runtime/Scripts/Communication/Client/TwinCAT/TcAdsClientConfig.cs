using System;
using UnityEngine;

namespace OC.Communication.TwinCAT
{
    [Serializable]
    public class TcAdsClientConfig
    {
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public bool Reconnect
        {
            get => _reconnect;
            set => _reconnect = value;
        }

        public string NetId
        {
            get => _netId;
            set => _netId = value;
        }

        public int Port
        {
            get => _port;
            set => _port = value;
        }

        public bool ClearBuffer
        {
            get => _clearBuffer;
            set => _clearBuffer = value;
        }

        public bool Verbose
        {
            get => _verbose;
            set => _verbose = value;
        }

        [SerializeField]
        private string _name;
        [SerializeField]
        private bool _reconnect;
        [SerializeField]
        private string _netId;
        [Tooltip("Port range [301...399], [851...899]")]
        [SerializeField]
        private int _port;
        [Tooltip("If Disconnected, process data will be overwrite with null")]
        [SerializeField]
        private bool _clearBuffer;
        [SerializeField]
        private bool _verbose;

        public static TcAdsClientConfig Default => new ()
        {
            Name = "Client",
            Reconnect = true,
            NetId = "Local",
            Port = 351,
            ClearBuffer = true,
            Verbose = false
        };
    }
}