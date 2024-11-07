using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OC.Communication.TwinCAT
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class TcType
    {
        public const string BIT = "BIT";
        // ReSharper disable once InconsistentNaming
        public const string BOOL = "BOOL";
        // ReSharper disable once InconsistentNaming
        public const string BYTE = "BYTE";
        // ReSharper disable once InconsistentNaming
        public const string USINT = "USINT";
        // ReSharper disable once InconsistentNaming
        public const string SINT = "SINT";
        // ReSharper disable once InconsistentNaming
        public const string WORD = "WORD";
        // ReSharper disable once InconsistentNaming
        public const string UINT = "UINT";
        // ReSharper disable once InconsistentNaming
        public const string INT = "INT";
        // ReSharper disable once InconsistentNaming
        public const string DWORD = "DWORD";
        // ReSharper disable once InconsistentNaming
        public const string UDINT = "UDINT";
        // ReSharper disable once InconsistentNaming
        public const string DINT = "DINT";
        // ReSharper disable once InconsistentNaming
        public const string REAL = "REAL";
        // ReSharper disable once InconsistentNaming
        public const string LWORD = "LWORD";
        // ReSharper disable once InconsistentNaming
        public const string LINT = "LINT";
        // ReSharper disable once InconsistentNaming
        public const string ULINT = "ULINT";
        // ReSharper disable once InconsistentNaming
        public const string LREAL = "LREAL";
        
        /// <summary>
        /// Dictionary of all existing types with name and bitSize.
        /// </summary>
        public static readonly Dictionary<string, int> TypeDictionary = new()
        {
            {BIT, 1},
            {BOOL, 8},
            {BYTE, 8 },
            {USINT, 8 },
            {SINT, 8 },
            {WORD, 16 },
            {UINT, 16 },
            {INT, 16 },
            {DWORD, 32 },
            {UDINT, 32 },
            {DINT, 32 },
            {REAL, 32 },
            {LWORD, 64 },
            {LINT, 64 },
            {ULINT, 64 },
            {LREAL, 64 }
        };

        /// <summary>
        /// Converts the TcType to a <see cref="T:System.Type"/>. Default is <see cref="T:System.Byte"/>[]
        /// </summary>
        public static Type TcTypeConvert(this string type)
        {
            switch (type)
            {
                case BIT:
                case BOOL:
                    return typeof(bool);
                case BYTE:
                case USINT:
                case SINT:
                    return typeof(byte);
                case WORD:
                case UINT:
                    return typeof(ushort);
                case INT:
                    return typeof(short);
                case DWORD:
                case UDINT:
                    return typeof(uint);
                case DINT:
                    return typeof(int);
                case LWORD:
                case ULINT:
                    return typeof(ulong);
                case LINT:
                    return typeof(long);
                case REAL:
                    return typeof(float);
                case LREAL:
                    return typeof(double);
                default:
                    return typeof(byte[]);
            }
        }
    }
}