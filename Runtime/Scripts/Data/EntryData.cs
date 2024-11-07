using System;
using UnityEngine;

namespace OC.Data
{
    [Serializable]
    public class EntryData
    {
        public EntryDataType Type
        {
            get => _type;
            set => _type = value;
        }

        public string Key 
        {
            get => _key;
            set => _key = value;
        }
        public string Value 
        {
            get => _value;
            set => _value = value;
        }

        [SerializeField]
        private EntryDataType _type = EntryDataType.CHARS;
        [SerializeField]
        private string _key = "New key";
        [SerializeField]
        private string _value = string.Empty;

        public EntryData(){}

        public EntryData(EntryDataType type, string key, string value)
        {
            _type = type;
            _key = key;
            _value = value;
        }

        public bool Validate(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return true; 
                }

                switch (_type)
                {
                    case EntryDataType.CHARS:
                        break;
                    case EntryDataType.BYTES:
                    
                        if (!IsHexString(value) || !value.StartsWith("0x"))
                        {
                            throw new Exception($"'{value}' is not a hex value");
                        }

                        var isEven = value.Length % 2 == 0;
                        if (!isEven)
                        {
                            throw new Exception($"'{value}' must have even length");
                        }
                        break;
                    case EntryDataType.INT16:
                        if (!short.TryParse(value, out _))
                        {
                            throw new Exception($"'{value}' cannot be parsed to INT16");
                        }
                        break;
                    case EntryDataType.INT32:
                        if (!int.TryParse(value, out _))
                        {
                            throw new Exception($"'{value}' cannot be parsed to INT32");
                        }
                        break;
                    case EntryDataType.UINT16:
                        if (!ushort.TryParse(value, out _))
                        {
                            throw new Exception($"'{value}' cannot be parsed to UINT16");
                        }
                        break;
                    case EntryDataType.UINT32:
                        if (!uint.TryParse(value, out _))
                        {
                            throw new Exception($"'{value}' cannot be parsed to UINT32");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return true;
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Warning, $"{_key}: {exception}");
                return false;
            }
        }
        
        public static bool IsHexString(string hex)
        {
            hex = hex.Replace("0x", "");

            try
            {
                var raw = new byte[hex.Length / 2];
                for (var i = 0; i < raw.Length; i++)
                {
                    raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
    
    public enum EntryDataType
    {
        // ReSharper disable once InconsistentNaming
        CHARS,
        // ReSharper disable once InconsistentNaming
        BYTES,
        // ReSharper disable once InconsistentNaming
        INT16,
        // ReSharper disable once InconsistentNaming
        INT32,
        // ReSharper disable once InconsistentNaming
        UINT16,
        // ReSharper disable once InconsistentNaming
        UINT32
    };
}
