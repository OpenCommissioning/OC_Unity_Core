#nullable enable
using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace OC.Editor.Assistant
{
    internal class ApiNode : IDisposable
    {
        private NamedPipeServerStream? _pipeServer;
        private readonly CancellationTokenSource _cancel = new();

        private readonly string _localPipeName;
        private readonly string _remotePipeName;
        private readonly string _remoteHostname;

        internal ApiNode(string localPipeName, string remotePipeName, string remoteHostname)
        {
            _localPipeName = localPipeName;
            _remotePipeName = remotePipeName;
            _remoteHostname = remoteHostname;
        }

        public void Listen()
        {
            Listen($"{_localPipeName}.server");
        }

        public Action<ApiMessage>? MessageReceived;

        public async Task<bool> Send(string data)
        {
            try
            {
                await using var pipeStream = new NamedPipeClientStream(
                    _remoteHostname,
                    $"{_remotePipeName}.server",
                    PipeDirection.InOut,
                    PipeOptions.Asynchronous);
                await pipeStream.ConnectAsync(100);
                var apiMessage = new ApiMessage(data);
                await pipeStream.WriteAsync(apiMessage.Buffer, 0, apiMessage.Buffer.Length);
                return true;
            }
            catch (Exception exception)
            {
                Logging.Logger.LogError(exception);
                return false;
            }
        }

        private void Listen(string pipeName)
        {
            var cancelToken = _cancel.Token;

            Task.Run(async () =>
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    _pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                    await _pipeServer.WaitForConnectionAsync(_cancel.Token);

                    var buffer = new byte[ApiMessage.HeaderSize];
                    if (await _pipeServer.ReadAsync(buffer, 0, ApiMessage.HeaderSize, cancelToken) == ApiMessage.HeaderSize
                        && ApiMessage.IsHeaderValid(buffer))
                    {
                        var size = ApiMessage.GetExpectedSize(buffer);
                        buffer = new byte[size];

                        if (await _pipeServer.ReadAsync(buffer, 0, size, cancelToken) == size)
                        {
                            MessageReceived?.Invoke(new ApiMessage(buffer));
                        }
                    }

                    _pipeServer?.Close();
                }
            }, cancelToken);
        }

        public void Dispose()
        {
            _cancel.Cancel();
            _pipeServer?.Close();
        }
    }
}