using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OC.Communication.TwinCAT
{
    public static class AdsStringExtension
    {
        private const string NAMESPACE = "OC.";

        /// <summary>
        /// Verifies if a TwinCAT type of this name exists.
        /// </summary>
        public static bool IsTcBaseType(this string type)
        {
            return TcType.TypeDictionary.ContainsKey(type);
        }
        
        /// <summary>
        /// Verifies if a TwinCAT type of this name exists.
        /// </summary>
        public static bool IsTcArrayType(this string type)
        {
            return type.ToLower().Contains("array");
        }
        
        /// <summary>
        /// Returns the TwinCAT bitSize of this type if exists, otherwise returns 0.
        /// </summary>
        public static int TcBitSize(this string type)
        {
            return TcType.TypeDictionary.TryGetValue(type, out var value) ? value : 0;
        }

        /// <summary>
        /// Removes the OC namespace from the sting.
        /// </summary>
        public static string RemoveTcNameSpace(this string type)
        {
            return type.StartsWith(NAMESPACE) ? type.Replace(NAMESPACE, "") : type;
        }

        /// <summary>
        /// Verifies if a custom TwinCAT type of this name exists.
        /// </summary>
        public static bool IsTcForceType(this string type)
        {
            // Remove namespace first
            var typeName = type.RemoveTcNameSpace();

            // String starts with underscore, rest of the string is a TcBaseType
            return typeName.StartsWith("_") && typeName.Remove(0, 1).IsTcBaseType();
        }

        /// <summary>
        /// Convert a string of hex values (e.g. 0xFF, 03-02-01, ef cd ab) into a byte array
        /// </summary>
        public static byte[] ToByteArray(this string value)
        {
            var hex = value.Replace(" ", "").Replace("0x", "").Replace("-", "");
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];
            for (var i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Convert a string of numbers (e.g. 1,2,3,4,16-32,64-128) into a list
        /// </summary>
        public static int[] ToNumberList(this string value)
        {
            var res = new List<int>();
            const string pattern = @"(?<val>((?<low>\d+)\s*?-\s*?(?<high>\d+))|(\d+))";

            foreach (Match m in Regex.Matches(value, pattern))
            {
                if (!m.Groups["val"].Success) continue;

                //found area (e.g. 16-32)
                if (m.Groups["low"].Success && m.Groups["high"].Success)
                {
                    var low = Convert.ToInt32(m.Groups["low"].Value);
                    var high = Convert.ToInt32(m.Groups["high"].Value);
                    for (var i = low; i <= high; i++)
                    {
                        res.Add(i);
                    }
                    continue; //next match
                }
                res.Add(Convert.ToInt32(m.Groups["val"].Value)); //single value

            }
            return res.ToArray();
        }
    }
}