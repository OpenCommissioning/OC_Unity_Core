using System;
using System.Text;

namespace OC.Editor.Assistant
{
    internal class ApiMessage
    {
        public byte[] Buffer { get; }
        public static int HeaderSize => 4;

        public ApiMessage(string payload)
        {
            Buffer = BuildMessage(Encoding.UTF8.GetBytes(payload));
        }

        public ApiMessage(byte[] payload)
        {
            Buffer = BuildMessage(payload);
        }

        public new string ToString()
        {
            return Encoding.UTF8.GetString(Buffer, HeaderSize, Buffer.Length - HeaderSize);
        }

        public static bool IsHeaderValid(byte[] header)
        {
            if (header.Length != HeaderSize)
            {
                return false;
            }

            return header[3] == (byte)(header[2] + (header[0] ^ header[1]));
        }

        public static int GetExpectedSize(byte[] header)
        {
            if (header.Length != HeaderSize)
            {
                return 0;
            }

            return header[0] << 8 | header[1];
        }

        private static byte[] BuildHeader(byte[] payload)
        {
            var header = new byte[HeaderSize];
            header[0] = (byte)((payload.Length & 0xFF00) >> 8);
            header[1] = (byte)(payload.Length & 0x00FF);
            header[2] = 0x01;
            header[3] = (byte)(header[2] + (header[0] ^ header[1]));
            return header;
        }

        private static byte[] BuildMessage(byte[] payload)
        {
            var header = BuildHeader(payload);
            var message = new byte[header.Length + payload.Length];
            Array.Copy(header, 0, message, 0, HeaderSize);
            Array.Copy(payload, 0, message, HeaderSize, payload.Length);
            return message;
        }
    }
}