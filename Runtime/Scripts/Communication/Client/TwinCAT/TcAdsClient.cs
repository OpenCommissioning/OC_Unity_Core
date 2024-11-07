using System;
using System.Xml.Linq;
using UnityEngine;
using AdsClient = TwinCAT.Ads.TcAdsClient;

namespace OC.Communication.TwinCAT
{
    [Serializable]
    [DefaultExecutionOrder(1000)]
    public class TcAdsClient : Client
    {
        public override IClientBuffer Buffer => _adsClientBuffer;
        public bool AutoConnect => _autoConnect;
        public string NetId => _netId;
        public int Port => _port;

        [Header("ADS")]
        [SerializeField]
        private bool _autoConnect;
        [SerializeField]
        private string _netId = "Local";
        [SerializeField]
        [Tooltip("Port range [301...399], [851...899]")]
        private int _port = 851;

        private TcClientStateHandler _clientStateHandler;
        private IClientBuffer _adsClientBuffer;

        private new void OnDisable()
        {
            base.OnDisable();
            Dispose();
        }
        
        private new void Start()
        {
            base.Start();
            _clientStateHandler = new TcClientStateHandler();
            _clientStateHandler.OnStateChanged += OnClientStateChanged;
            _clientStateHandler.Connect(_netId);
        }

        public override void BeforeFixedUpdate()
        {
            Read();
            base.BeforeFixedUpdate();
        }
        
        public override void AfterFixedUpdate()
        {
            base.AfterFixedUpdate();
            Write();
        }
        
        private void OnClientStateChanged(TcAdsExtension.TcClientState state)
        {
            if (!_autoConnect) return;

            switch (state)
            {
                case TcAdsExtension.TcClientState.Stop:
                    Logging.Logger.Log("TcAdsClient <color=red>STOPPED</color>");
                    Disconnect();
                    break;
                case TcAdsExtension.TcClientState.Run:
                    Logging.Logger.Log("TcAdsClient <color=green>RUNNING</color>");
                    Connect();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public override void Connect()
        {
            if (_isConnected.Value) return;

            try
            {
                _ = ValidatePort(_port);
                
                if (!_clientStateHandler.IsRunning)
                {
                    throw new Exception("Target system isn't in run mode");
                }

                var tcAdsBufferType = GetTcAdsBufferType(_port);

                switch (tcAdsBufferType)
                {
                    case TcAdsBufferType.None:
                        throw new Exception($"Port:{_port} is out of range [301...399], [851...899]");
                    case TcAdsBufferType.Array:
                        _adsClientBuffer = new TcAdsTaskBuffer(this);
                        _adsClientBuffer.Connect(_netId, _port);
                        break;
                    case TcAdsBufferType.SumCommand:
                        _adsClientBuffer = new TcAdsSumCommandBuffer(this);
                        _adsClientBuffer.Connect(_netId, _port);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _isConnected.Value = true;
            }
            catch (Exception exception)
            {
                _isConnected.Value = false;
                Logging.Logger.Log(LogType.Error, $"TcAdsClient failed to connect: {_netId}:{_port}. {exception.Message}. \n{exception.Source}{exception.StackTrace}");
            }
        }

        public override void Disconnect()
        {
            if (!_isConnected.Value) return;
            
            _isConnected.Value = false;
            
            try
            {
                _adsClientBuffer.Disconnect();
            }
            catch (Exception)
            {
                //ignore
            }
        }
        
        private void Dispose()
        {
            Disconnect();
            _clientStateHandler.OnStateChanged -= OnClientStateChanged;
            _clientStateHandler.Dispose();
        }
        
        private void Read()
        {
            if (!_isConnected.Value) return;
            
            try
            {
                _adsClientBuffer.Read();
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, $"{exception.Message}\n {exception.Source}\n {exception.StackTrace}");
                Disconnect();
            }
        }

        private void Write()
        {
            if (!_isConnected.Value) return;

            try
            {
                _adsClientBuffer.Write();
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, $"{exception.Message}\n {exception.Source}\n {exception.StackTrace}");
                Disconnect();
            }
        }

        private enum TcAdsBufferType
        {
            None,
            Array,
            SumCommand
        }
        
        private bool ValidatePort(int port)
        {
            return port switch
            {
                > 300 and < 400 => true,
                > 850 and < 900 => true,
                _ => throw new Exception($"Port:{_port} is out of range [301...399], [851...899]")
            };
        }

        private TcAdsBufferType GetTcAdsBufferType(int port)
        {
            return port switch
            {
                > 300 and < 400 => TcAdsBufferType.Array,
                > 850 and < 900 => TcAdsBufferType.SumCommand,
                _ => TcAdsBufferType.None
            };
        }
        
        public override XElement GetAsset()
        {
            var config = new TcAdsClientConfig(this);
            var element = config.ToXElement<TcAdsClientConfig>();
            element.SetAttributeValue("Name", name);
            return element;
        }

        public override void SetAsset(XElement xElement)
        {
            try
            {
                var config = xElement.FromXElement<TcAdsClientConfig>();
                _autoConnect = config.Reconnect;
                _netId = config.NetId;
                _port = config.Port;
                _verbose = config.Verbose;
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, exception);
                throw;
            }
        }
    }
}
