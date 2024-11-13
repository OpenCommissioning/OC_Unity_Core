using System;
using System.Collections.Generic;
using System.Linq;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;
using UnityEngine;
using AdsClient = TwinCAT.Ads.TcAdsClient;

namespace OC.Communication.TwinCAT
{
    public class TcAdsTaskBuffer : IClientBuffer
    {
        public bool IsConnected => _isConnected;
        public byte[] InputBytes => _inputBuffer;
        public byte[] OutputBytes => _outputBuffer;
        public List<ClientVariable> InputVariables => _inputVariables;
        public List<ClientVariable> OutputVariables => _outputVariables;
        
        private readonly List<ClientVariable> _inputVariables = new();
        private readonly List<ClientVariable> _outputVariables = new();
        private readonly List<IAdsSymbol> _inputSymbols = new();
        private readonly List<IAdsSymbol> _outputSymbols = new();
        private readonly Client _client;
        private AdsClient _adsClient;
        private string _netId;
        private int _port;
        private uint _addrGroup;
        private const uint ADDR_OFFSET_INPUTS = 0x80000000;
        private const uint ADDR_OFFSET_OUTPUTS = 0x81000000;
        private byte[] _inputBuffer;
        private byte[] _outputBuffer;
        private bool _isConnected;
        
        public TcAdsTaskBuffer(Client client)
        {
            _client = client;
        }

        public void Connect(string netId, int port)
        {
            _isConnected = false;
            _netId = netId;
            _port = port;

            _adsClient = new AdsClient();
            _adsClient.Connect(_netId, _port);

            if (!_adsClient.IsConnected) throw new Exception($"TcAds Client can't connect to {_netId}:{_port}");
            Initialize();
            Logging.Logger.Log(LogType.Log, $"TcAds Task Client is <color=green>connected</color> to {_netId}:{_port}");
        }

        public void Disconnect()
        {
            WriteNullData();
            Clear();
            _adsClient?.Disconnect();
            _isConnected = false;
            Logging.Logger.Log(LogType.Log, $"TcAds Task Client is <color=red>disconnected</color> from {_netId}:{_port}");
        }
        
        public void Read()
        {
            if (!_isConnected) return;
            if (!_adsClient.IsConnected) return;
            if (_inputBuffer.Length == 0) return;
            
            _adsClient.Read(_addrGroup, ADDR_OFFSET_INPUTS, _inputBuffer, 0, _inputBuffer.Length);
        }
        
        public void Write()
        {
            if (!_isConnected) return;
            if (!_adsClient.IsConnected) return;
            if (_inputBuffer.Length == 0) return;
            
            _adsClient.Write(_addrGroup, ADDR_OFFSET_OUTPUTS, _outputBuffer, 0, _outputBuffer.Length);
        }
        
        private void Initialize()
        {
            var symbolLoaderSettings = new SymbolLoaderSettings(SymbolsLoadMode.Flat);
            var symbolLoader = (IAdsSymbolLoader)SymbolLoaderFactory.Create(_adsClient, symbolLoaderSettings);

            if (symbolLoader.Symbols.Count <= 0)
            {
                throw new Exception("SymbolLoader.Symbols.Count = 0");
            }
            
            var adsSymbol = (IAdsSymbol)symbolLoader.Symbols[0];
            _addrGroup = adsSymbol.IndexGroup;

            AddSymbols(symbolLoader);
            
            _inputBuffer = new byte[_inputSymbols.Sum(item => item.ByteSize)];
            _outputBuffer = new byte[_outputSymbols.Sum(item => item.ByteSize)];

            CreateVairables(_inputBuffer, _inputSymbols, _inputVariables);
            CreateVairables(_outputBuffer, _outputSymbols, _outputVariables);

            _isConnected = true;
            Logging.Logger.Log(LogType.Log, $"TcAds Task Client {_inputBuffer.Length} byte inputs - {_outputBuffer.Length} byte outputs.");
        }
        
        private void AddSymbols(ISymbolProvider adsSymbolLoader)
        {
            foreach (var symbol in adsSymbolLoader.Symbols)
            {
                AddSymbol((IAdsSymbol)symbol);
            }
            
            _inputSymbols.Sort((x,y) => x.IndexOffset.CompareTo(y.IndexOffset));
            _outputSymbols.Sort((x,y) => x.IndexOffset.CompareTo(y.IndexOffset));
        }

        private void CreateVairables(byte[] buffer, List<IAdsSymbol> symbols, List<ClientVariable> variables)
        {
            var offset = 0;
            
            foreach (var symbol in symbols)
            {
                var instancePath = symbol.InstancePath.Split(".");
                var split = instancePath.ToList().Skip(2);
                var name = string.Join(".", split.ToArray());
                var length = symbol.Size;
                
                variables.Add(new ClientVariable(buffer, name, length, offset));
                offset += length;
            }
        }

        private void WriteNullData()
        {
            if (!_isConnected) return;
            if (!_adsClient.IsConnected) return;
            var empty = new byte[_outputBuffer.Length];
            _adsClient.Write(_addrGroup, ADDR_OFFSET_OUTPUTS, empty, 0, _outputBuffer.Length);
        }
        
        private void Clear()
        {
            _inputVariables.Clear();
            _outputVariables.Clear();
            _inputSymbols.Clear();
            _outputSymbols.Clear();
            _isConnected = false;
        }

        private void AddSymbol(IAdsSymbol symbol)
        {
            try
            {
                var split = symbol.InstancePath.Split(".");
                if (split.Length < 3)
                {
                    throw new Exception($"IAdsSymbol isn't valid!. Split of {symbol.InstancePath} is < 3");
                }

                var direction = split[1] == "Outputs" ? ClientVariableDirection.Output : ClientVariableDirection.Input;

                switch (direction)
                {
                    case ClientVariableDirection.Input:
                        _inputSymbols.Add(symbol);
                        break;
                    case ClientVariableDirection.Output:
                        _outputSymbols.Add(symbol);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                if (_client.Verbose) Logging.Logger.Log(LogType.Log, Logging.Logger.VERBOSE_TAG + " <color=green>Find ADS Variable:</color>" + $"{symbol.InstancePath} : {symbol.TypeName} : {direction}");
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, exception.Message);
            }
        }
    }
}