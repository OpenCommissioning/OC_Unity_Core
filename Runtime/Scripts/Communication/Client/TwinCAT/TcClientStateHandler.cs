using System;
using TwinCAT.Ads;
using AdsClient = TwinCAT.Ads.TcAdsClient;

namespace OC.Communication.TwinCAT
{
    public class TcClientStateHandler
    {
        public bool IsRunning => _state == TcAdsExtension.TcClientState.Run;

        public TcAdsExtension.TcClientState State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                _state = value;
                OnStateChanged?.Invoke(_state);
            }
        }

        public Action<TcAdsExtension.TcClientState> OnStateChanged;
        private readonly AdsClient _adsClient;
        private TcAdsExtension.TcClientState _state;

        public TcClientStateHandler()
        {
            _adsClient = new AdsClient();
        }

        public void Connect(string netId)
        {
            try
            {
                _adsClient.Connect(netId, (int)AmsPort.R0_Realtime);
                _adsClient.AmsRouterNotification += OnAmsRouterNotification;
                _state = _adsClient.GetTcClientState();
                OnStateChanged?.Invoke(_state);
            }
            catch (Exception exception)
            {
                Logging.Logger.LogError(exception);
                throw;
            }
        }

        private void OnAmsRouterNotification(object sender, AmsRouterNotificationEventArgs e)
        {
            var isRunning = e.State == AmsRouterState.Start;
            State = isRunning ? _adsClient.GetTcClientState() : TcAdsExtension.TcClientState.Stop;
        }
        
        public void Dispose()
        {
            _adsClient.Dispose();
        }
    }
}
