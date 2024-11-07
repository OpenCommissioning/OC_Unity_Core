using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwinCAT.Ads;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;

namespace OC.Communication.TwinCAT   
{
    public static class TcAdsExtensionForDeviceView
    {
        /// <summary>
        /// Symbols filtered by {attribute 'unity-ui'}
        /// </summary>
        /// <returns>A dictionary from type </returns>
        public static Dictionary<string, TcAdsDevice> FilteredSymbols(this ISymbolProvider symbolProvider)
        {
            var dict = new Dictionary<string, TcAdsDevice>();

            foreach (var symbol in symbolProvider.Symbols)
            {
                symbol.FilteredSubSymbols(dict);
            }

            return dict;
        }

        private static void FilteredSubSymbols(this ISymbol symbol, IDictionary<string, TcAdsDevice> dict)
        {
            if (symbol.Category == DataTypeCategory.Array) return;

            //Symbol has attribute 'unity-ui'
            if (symbol.Attributes.Any(x => x.Name.ToLower() == "unity-ui"))
            {
                var devicePath = symbol.Parent.InstancePath;
                var deviceType = symbol.Parent.TypeName;

                if (!dict.ContainsKey(devicePath))
                    dict.Add(devicePath, new TcAdsDevice(devicePath, deviceType));

                var device = dict[devicePath];

                symbol.AddToCollection(device.Symbols);
            }

            //Recursive call 
            foreach (var sub in symbol.SubSymbols)
            {
                sub.FilteredSubSymbols(dict);
            }
        }

        private static void AddToCollection(this ISymbol symbol, ICollection<ISymbol> symbols)
        {
            if (symbol.Category == DataTypeCategory.Pointer)
            {
                return;
            }

            if (symbol.Category == DataTypeCategory.Primitive || symbol.Category == DataTypeCategory.Enum)
            {
                symbols.Add(symbol);
                return;
            }

            //Recursive call 
            foreach (var subSymbol in symbol.SubSymbols)
            {
                subSymbol.AddToCollection(symbols);
            }
        }

        /// <summary>
        /// Write a value to the given symbol.
        /// </summary>
        public static void SetValue(this IAdsAnyAccess client, ISymbol symbol, object value)
        {
            var adsSymbol = (IAdsSymbol)symbol;
            var typeName = symbol.TypeName.ToUpper();

            //Don't overwrite pointers
            if (symbol.Category == DataTypeCategory.Pointer) return;

            client.WriteAny(adsSymbol.IndexGroup, adsSymbol.IndexOffset, value.ConvertTo(typeName.TcTypeConvert()));
        }

        /// <summary> 
        /// Read the value from the given symbol.
        /// </summary>
        public static object GetValue(this IAdsAnyAccess client, ISymbol symbol)
        {
            var adsSymbol = (IAdsSymbol)symbol;

            if (symbol.TypeName.IsTcBaseType())
                return client.ReadAny(adsSymbol.IndexGroup, adsSymbol.IndexOffset, symbol.TypeName.TcTypeConvert());

            var byteArray = (byte[])client.ReadAny(adsSymbol.IndexGroup, adsSymbol.IndexOffset, typeof(byte[]), new[] { symbol.ByteSize });
            Array.Reverse(byteArray);
            return $"0x{BitConverter.ToString(byteArray).Replace("-", "")}";
        }

        /// <summary>
        /// Read all given symbols in one read request.
        /// </summary>
        public static BinaryReader BlockRead(this IAdsReadWriteAccess client, IEnumerable<ISymbol> symbols)
        {
            var list = symbols.ToList();

            // Stream size
            var errorCodesLength = list.Count * 4;
            var rdLength = errorCodesLength;
            var wrLength = list.Count * 12;

            // Write data for handles into the ADS Stream
            var writer = new BinaryWriter(new AdsStream(wrLength));

            foreach (var adsSymbol in list.Cast<IAdsSymbol>())
            {
                writer.Write(adsSymbol.IndexGroup);
                writer.Write(adsSymbol.IndexOffset);
                writer.Write(adsSymbol.ByteSize);
                rdLength += adsSymbol.ByteSize;
            }

            // Sum command to read variables from the PLC
            var rdStream = new AdsStream(rdLength);
            client.ReadWrite(0xF080, (uint)list.Count, rdStream, (AdsStream)writer.BaseStream);

            //BinaryReader from current ADS stream
            var reader = new BinaryReader(rdStream);

            //Set current position to first value
            _ = reader.ReadBytes(errorCodesLength);
            return reader;
        }

        /// <summary>
        /// Reset all given symbols in one write request.
        /// </summary>
        public static void BlockReset(this IAdsReadWriteAccess client, IEnumerable<ISymbol> symbols)
        {
            var list = symbols.ToList();
            var dataLength = list.Sum(sym => sym.ByteSize);

            // Stream size
            var rdLength = list.Count * 4;
            var wrLength = list.Count * 12 + dataLength;

            // Write data for handles into the ADS Stream
            var writer = new BinaryWriter(new AdsStream(wrLength));

            foreach (var adsSymbol in list.Cast<IAdsSymbol>())
            {
                writer.Write(adsSymbol.IndexGroup);
                writer.Write(adsSymbol.IndexOffset);
                writer.Write(adsSymbol.ByteSize);
            }

            // Sum command to write variables to the PLC
            var rdStream = new AdsStream(rdLength);
            client.ReadWrite(0xF081, (uint)list.Count, rdStream, (AdsStream)writer.BaseStream);
        }

        /// <summary> 
        /// Read the value with length and type of the given symbol.
        /// </summary>
        public static object ReadValue(this BinaryReader binaryReader, ISymbol symbol)
        {
            switch (symbol.TypeName)
            {
                case TcType.BIT:
                case TcType.BOOL:
                    return binaryReader.ReadBoolean();
                case TcType.BYTE:
                case TcType.USINT:
                case TcType.SINT:
                    return binaryReader.ReadByte();
                case TcType.WORD:
                case TcType.UINT:
                    return binaryReader.ReadUInt16();
                case TcType.INT:
                    return binaryReader.ReadInt16();
                case TcType.DWORD:
                case TcType.UDINT:
                    return binaryReader.ReadUInt32();
                case TcType.DINT:
                    return binaryReader.ReadInt32();
                case TcType.LWORD:
                case TcType.ULINT:
                    return binaryReader.ReadUInt64();
                case TcType.LINT:
                    return binaryReader.ReadInt64();
                case TcType.REAL:
                    return binaryReader.ReadSingle();
                case TcType.LREAL:
                    return binaryReader.ReadDouble();
                default:
                    var byteArray = binaryReader.ReadBytes(symbol.ByteSize);
                    Array.Reverse(byteArray);
                    return $"0x{BitConverter.ToString(byteArray).Replace("-", "")}";
            }
        }
    }
}