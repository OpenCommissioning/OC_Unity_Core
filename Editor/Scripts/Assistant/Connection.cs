using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OC.Editor.Assistant
{
    /// <summary>
    /// Connection to an Assistant instance based on a <see cref="System.IO.Pipes.NamedPipeServerStream"/>.
    /// </summary>
    public class Connection : IDisposable
    {
        private const string CONFIG_IDENTIFIER = "cfg";
        private const string MESSAGE_IDENTIFIER = "msg";
        private const string TIMESCALE_IDENTIFIER = "tsc";

        private static readonly string AppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private readonly ApiNode _apiNode;

        /// <summary>
        /// Creates a new connection to the Assistant.
        /// </summary>
        public Connection() : this(".")
        {
        }

        private Connection(string remoteHostName)
        {
            _apiNode = new ApiNode(
                $"{nameof(OC)}.{nameof(Assistant)}.Remote",
                $"{nameof(OC)}.{nameof(Assistant)}",
                remoteHostName);

            _apiNode.MessageReceived += OnReceived;
            _apiNode.Listen();
        }

        /// <summary>
        /// Sends a client configuration to the Assistant.
        /// <param name="config">The configuration to be sent.</param>
        /// </summary>
        public async Task SendConfigAsync(XElement config)
        {
            var filePath = $@"{AppDataFolder}\{nameof(OC)}.{nameof(Assistant)}\ClientConfig.xml";
            config.Save(filePath);
            await _apiNode.Send($"{CONFIG_IDENTIFIER}{filePath}");
        }

        /// <summary>
        /// Sends a TimeScaling value to the Assistant.
        /// </summary>
        /// <param name="value">The TimeScaling value to be sent.</param>
        public async Task SendTimeScalingAsync(double value)
        {
            await _apiNode.Send($"{TIMESCALE_IDENTIFIER}{value}");
        }

        /// <summary>
        /// Sends a command to the Assistant.
        /// <param name="message">The message to be sent.</param>
        /// </summary>
        public async Task SendMessageAsync(string message)
        {
            await _apiNode.Send($"{MESSAGE_IDENTIFIER}{message}");
        }

        /// <summary>
        /// Received a message from the Assistant.
        /// </summary>
        public event Action<string> MessageReceived;

        private void OnReceived(ApiMessage apiMessage)
        {
            var message = apiMessage.ToString();

            if (message[..3] != MESSAGE_IDENTIFIER)
            {
                return;
            }

            MessageReceived?.Invoke(message[3..]);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _apiNode.Dispose();
        }
    }
}