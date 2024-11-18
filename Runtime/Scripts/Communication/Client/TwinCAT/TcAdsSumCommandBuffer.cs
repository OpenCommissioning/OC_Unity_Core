using System;
using System.Collections.Generic;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using UnityEngine;
using AdsClient = TwinCAT.Ads.TcAdsClient;

namespace OC.Communication.TwinCAT
{
    public class TcAdsSumCommandBuffer : IClientBuffer
    {
        public bool IsConnected => _isConnected;
        public byte[] InputBytes => _blockWriteWrStream.Buffer;
        public byte[] OutputBytes => _blockReadRdStream.Buffer;
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
        private ByteStream _blockReadWrStream;
        private ByteStream _blockReadRdStream;
        private ByteStream _blockWriteWrStream;
        private ByteStream _blockWriteRdStream;
        private int _inputSize;
        private int _outputSize;
        
        private bool _isConnected;
        
        public TcAdsSumCommandBuffer(Client client)
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
            if (_inputSymbols.Count == 0) return;
            
            _adsClient.ReadWrite(
                0xF080, 
                (uint)_inputSymbols.Count,
                _blockReadRdStream.Buffer,
                0, 
                _blockReadRdStream.Length,
                _blockReadWrStream.Buffer,
                0,
                _blockReadWrStream.Length);
        }
        
        public void Write()
        {
            if (!_isConnected) return;
            if (!_adsClient.IsConnected) return;
            if (_outputSymbols.Count == 0) return;
            
            _adsClient.ReadWrite(
                0xF081, 
                (uint)_outputSymbols.Count, 
                _blockWriteRdStream.Buffer,
                0, 
                _blockWriteRdStream.Length,
                _blockWriteWrStream.Buffer,
                0,
                _blockWriteWrStream.Length);
        }

        private void Initialize()
        {
            _inputSize = 0;
            _outputSize = 0;
            var symbolLoaderSettings = new SymbolLoaderSettings(SymbolsLoadMode.Flat);
            var symbolLoader = (IAdsSymbolLoader)SymbolLoaderFactory.Create(_adsClient, symbolLoaderSettings);
            var symbols = symbolLoader.Symbols;
            
            if (symbols.Count <= 0)
            {
                throw new Exception("SymbolLoader.Symbols.Count = 0!");
            }

            foreach (var symbol in symbols)
            {
                if (!symbol.InstancePath.Contains(_client.RootName)) continue;
                if (!symbol.Attributes.TryGetAttribute("simulation_interface", out _)) continue;
                if (!symbol.Attributes.TryGetAttribute("TcAddressType", out var attribute)) continue;
                
                switch (attribute.Value.ToLower())
                {
                    case "output":
                    {
                        _inputSymbols.Add((IAdsSymbol)symbol);
                        _inputSize += symbol.ByteSize;
                        if (_client.Verbose) Logging.Logger.Log(LogType.Log, Logging.Logger.VERBOSE_TAG + " <color=green>Find ADS Variable:</color>" + $"{symbol.InstancePath} : {symbol.TypeName} : TcOutput");
                        break;
                    }
                    case "input":
                    {
                        _outputSymbols.Add((IAdsSymbol)symbol);
                        _outputSize += symbol.ByteSize;
                        if (_client.Verbose) Logging.Logger.Log(LogType.Log, Logging.Logger.VERBOSE_TAG + " <color=green>Find ADS Variable:</color>" + $"{symbol.InstancePath} : {symbol.TypeName} : TcInput");
                        break;
                    }
                }
            }

            InitializeSumCommandBuffer();

            _isConnected = true;
            Logging.Logger.Log(LogType.Log, $"TcAds Client {_inputSize} byte inputs - {_outputSize} byte outputs.");
        }

        private void InitializeSumCommandBuffer()
        {
            _blockReadWrStream = new ByteStream(_inputSymbols.Count * 12);
            _blockReadRdStream = new ByteStream(_inputSymbols.Count * 4 + _inputSize);

            var offset = _inputSymbols.Count * 4;
            foreach (var adsSymbol in _inputSymbols)
            {
                _blockReadWrStream.Write(adsSymbol.IndexGroup);
                _blockReadWrStream.Write(adsSymbol.IndexOffset);
                _blockReadWrStream.Write(adsSymbol.ByteSize);
                _inputVariables.Add(new ClientVariable(_blockReadRdStream.Buffer, adsSymbol.InstancePath, adsSymbol.Size, offset));
                offset += adsSymbol.ByteSize;
            }

            _blockWriteWrStream = new ByteStream(_outputSymbols.Count * 12 + _outputSize);
            _blockWriteRdStream = new ByteStream(_outputSymbols.Count * 4);

            offset = _outputSymbols.Count * 12;
            foreach (var adsSymbol in _outputSymbols)
            {
                _blockWriteWrStream.Write(adsSymbol.IndexGroup);
                _blockWriteWrStream.Write(adsSymbol.IndexOffset);
                _blockWriteWrStream.Write(adsSymbol.ByteSize);
                _outputVariables.Add(new ClientVariable(_blockWriteWrStream.Buffer, adsSymbol.InstancePath, adsSymbol.Size, offset));
                offset += adsSymbol.ByteSize;
            }
        }

        private void WriteNullData()
        {
            if (!_isConnected) return;
            if (!_adsClient.IsConnected) return;
            Buffer.BlockCopy(new byte[_outputSize],0,_blockWriteWrStream.Buffer,_outputSymbols.Count * 12,_outputSize);
            _adsClient.ReadWrite(
                0xF081, 
                (uint)_outputSymbols.Count, 
                _blockWriteRdStream.Buffer,
                0, 
                _blockWriteRdStream.Length,
                _blockWriteWrStream.Buffer,
                0,
                _blockWriteWrStream.Length);
        }
        
        private void Clear()
        {
            _inputVariables.Clear();
            _outputVariables.Clear();
            _inputSymbols.Clear();
            _outputSymbols.Clear();
            _isConnected = false;
        }
    }
}