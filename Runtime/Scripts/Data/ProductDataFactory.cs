using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using OC.MaterialFlow;
using UnityEngine;

namespace OC.Data
{
    public static class ProductDataFactory
    {
        public const string DATA_EXTENSION = "data";
        private const string TAG = "<color=#3699ff>Product Data Manager</color>";
        private const string TEMPLATE_NAME = "Type";
        private const string DATA_NAME = "ID";
        private const string TAG_UNIQUE_ID = "UniqueId";
        

        public static void CreateProductData(this PayloadTag payloadTag, Dictionary<string, string> content = null)
        {
            try
            {
                foreach (var directoryId in payloadTag.DirecotryId)
                {
                    if (!ProductDataDirectoryManager.Instance.Contains(directoryId))
                    {
                        Logging.Logger.Log(LogType.Warning, $"{TAG} DirectoryId [{directoryId}] isn't contains in Directory Manager! Payload Product Data isn't created!");
                        continue;
                    }
                    string directory;
                    
                    if (Path.IsPathRooted(ProductDataDirectoryManager.Instance.ProductDataDirectories[directoryId].Path))
                    {
                        directory = ProductDataDirectoryManager.Instance.ProductDataDirectories[directoryId].Path;
                    }
                    else
                    {
                        directory = $"{Application.dataPath}\\{ProductDataDirectoryManager.Instance.ProductDataDirectories[directoryId].Path}";
                    }
                    
                    CreateProductDataFile(payloadTag, directory, content); 
                }
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, exception.Message);
            }
        }

        public static List<EntryData> GetProductDataContent(this PayloadTag payloadTag, int directoryIndex)
        {
            return GetProductDataContent(payloadTag, ProductDataDirectoryManager.Instance.ProductDataDirectories[directoryIndex].Path);
        }

        public static List<EntryData> GetProductDataContent(this PayloadTag payloadTag, ProductDataDirectory directory)
        {
            return GetProductDataContent(payloadTag, directory.Path);
        }
        
        public static void OverwriteProductData(this PayloadTag payloadTag, int directoryIndex, List<EntryData> data)
        {
            Overwrite(payloadTag, ProductDataDirectoryManager.Instance.ProductDataDirectories[directoryIndex].Path, data);
        }

        public static void OverwriteProductData(this PayloadTag payloadTag, ProductDataDirectory directory, List<EntryData> data)
        {
            Overwrite(payloadTag, directory.Path, data);
        }
        
        public static void CreateTemplate(string path, List<EntryData> content)
        {
            if (string.IsNullOrEmpty(path)) return;
            
            var elem = new XElement(TEMPLATE_NAME);

            foreach (var entry in content)
            {
                elem.Add(new XElement("Entry",
                    new XAttribute("Type", entry.Type),
                    new XAttribute("Key", entry.Key),
                    entry.Value));
            }
            
            new XDocument(elem).Save(path);
            Logging.Logger.Log(LogType.Log, $"{TAG} Template saved as {path}");
        }

        public static List<EntryData> GetTemplateContent(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var data = new List<EntryData>();
            var doc = XDocument.Load(path);
            if (doc.Root == null)
            {
                return null;
            }

            foreach (var entry in doc.Root.Elements("Entry"))
            {
                if (!Enum.TryParse<EntryDataType>(entry.Attribute("Type")?.Value, out var entryDataType)) continue;

                var entryData = new EntryData(entryDataType, entry.Attribute("Key")?.Value, entry.Value);
                data.Add(entryData);
            }

            return data;
        }

        private static List<EntryData> GetProductDataContent(this PayloadTag payloadTag, string directory)
        {
            try
            {
                var templatePath = Path.Combine(directory, $"{TEMPLATE_NAME}{payloadTag.Payload.TypeId}.xml");
                if (!File.Exists(templatePath)) throw new Exception($"{TAG} Template Data {templatePath} cannot be found!");
                
                var template = XDocument.Load(templatePath);
                var file = Path.Combine(directory, $"{DATA_NAME}{payloadTag.Payload.UniqueId}.{DATA_EXTENSION}");
                if (!File.Exists(file)) throw new Exception($"{TAG} Product Data {file} cannot be found!");
                
                var bytes = File.ReadAllBytes(file);
                var document = ReadDataBytes(template, bytes);
                var productData = new List<EntryData>();
                
                foreach (var entry in document.Elements("Entry"))
                {
                    if (!Enum.TryParse<EntryDataType>(entry.Attribute("Type")?.Value, out var entryDataType)) continue;
                    var entryData = new EntryData(entryDataType, entry.Attribute("Key")?.Value, entry.Value);
                    productData.Add(entryData);
                }

                return productData;
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, exception.Message);
                return null;
            }
        }

        private static void CreateProductDataFile(PayloadTag payloadTag, string directory, Dictionary<string, string> value = null)
        {
            Dictionary<string, string> content;

            if (value == null)
            {
                content = new Dictionary<string, string> { {TAG_UNIQUE_ID, payloadTag.Payload.UniqueId.ToString()} };
            }
            else
            {
                content = new Dictionary<string, string>(value);
                if (content.ContainsKey(TAG_UNIQUE_ID)) content[TAG_UNIQUE_ID] = payloadTag.Payload.UniqueId.ToString();
            }

            var path = Path.Combine(directory, $"{TEMPLATE_NAME}{payloadTag.Payload.TypeId}.xml");

            try
            {
                if (!File.Exists(path)) throw new Exception($"{TAG} Product data template not found! {path}");
                var template = XDocument.Load(path);
                var bytes = CreateTemplate(template, content);
                var file = Path.Combine(directory, payloadTag.GetProductDataPath());
                File.WriteAllBytes(file, bytes);
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, exception.Message, payloadTag);
            }
        }

        private static void Overwrite(PayloadTag payloadTag, string directory, List<EntryData> productData)
        {
            var elem = new XElement(TEMPLATE_NAME);
            
            foreach (var entry in productData)
            {
                elem.Add(new XElement("Entry",
                    new XAttribute("Type", entry.Type),
                    new XAttribute("Key", entry.Key),
                    entry.Value));
            }

            var path = Path.Combine(directory, $"{DATA_NAME}{payloadTag.Payload.UniqueId}.{DATA_EXTENSION}");

            try
            {
                if (!File.Exists(path)) throw new Exception($"{TAG} Product Data {path} cannot be found!");
                var template = new XDocument(elem);
                var dictinary = new Dictionary<string, string> { { "Number", payloadTag.Payload.UniqueId.ToString() } };
                var bytes = CreateTemplate(template, dictinary);
                File.WriteAllBytes(path, bytes);
                Logging.Logger.Log(LogType.Log, $"{TAG} Product Data {path} successfully overwritten!");
                
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, exception.Message);
            }
        }
        
        private static string GetProductDataPath(this PayloadTag payloadTag) => $"{DATA_NAME}{payloadTag.Payload.UniqueId}.{DATA_EXTENSION}";

        private static XElement ReadDataBytes(XDocument structure, byte[] data)
        {
            var buffer = new byte[32000];
            Buffer.BlockCopy(data, 0, buffer, 0, data.Length);
            var offset = 0;

            if (structure.Root == null)
            {
                return null;
            }
            
            foreach (var entry in structure.Root.Elements("Entry"))
            {
                var type = (EntryDataType)Enum.Parse(typeof(EntryDataType), entry.Attribute("Type")?.Value.ToUpper() ?? string.Empty);
                int length;

                switch (type)
                {
                    case EntryDataType.CHARS:
                        length = entry.Value.Length;
                        entry.Value = Encoding.ASCII.GetString(buffer, offset, length);
                        break;
                    case EntryDataType.BYTES:
                        length = entry.Value.Replace("0x", "").Length / 2;
                        entry.Value = "0x" + BitConverter.ToString(buffer, offset, length).Replace("-", "");
                        break;
                    case EntryDataType.UINT16:
                        length = 2;
                        entry.Value = BitConverter.ToUInt16(buffer, offset).ToString();
                        break;
                    case EntryDataType.UINT32:
                        length = 4;
                        entry.Value = BitConverter.ToUInt32(buffer, offset).ToString();
                        break;
                    case EntryDataType.INT16:
                        length = 2;
                        entry.Value = BitConverter.ToInt16(buffer, offset).ToString();
                        break;
                    case EntryDataType.INT32:
                        length = 4;
                        entry.Value = BitConverter.ToInt32(buffer, offset).ToString();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                offset += length;
            }

            return structure.Root;
        }

        private static byte[] CreateTemplate(XDocument template, Dictionary<string, string> content)
        {
            var data = new byte[32000];
            var offset = 0;

            if (template.Root == null)
            {
                return null;
            }
            
            foreach (var entry in template.Root.Elements("Entry"))
            {
                var type = (EntryDataType)Enum.Parse(typeof(EntryDataType), entry.Attribute("Type")?.Value.ToUpper() ?? string.Empty);
                var bytes = new byte[1];

                if (entry.Attribute("Key") != null)
                {
                    var key = entry.Attribute("Key")?.Value;
                    if (key != null && content.ContainsKey(key))
                        if (content[key].Length == entry.Value.Length || type != EntryDataType.CHARS)
                            entry.Value = content[key];
                }

                entry.Value = entry.Value == "" ? "0" : entry.Value;

                bytes = type switch
                {
                    EntryDataType.CHARS => Encoding.ASCII.GetBytes(entry.Value),
                    EntryDataType.BYTES => FromHex(entry.Value),
                    EntryDataType.INT16 => BitConverter.GetBytes(short.Parse(entry.Value)),
                    EntryDataType.INT32 => BitConverter.GetBytes(int.Parse(entry.Value)),
                    EntryDataType.UINT16 => BitConverter.GetBytes(ushort.Parse(entry.Value)),
                    EntryDataType.UINT32 => BitConverter.GetBytes(uint.Parse(entry.Value)),
                    _ => bytes
                };

                Buffer.BlockCopy(bytes, 0, data, offset, bytes.Length);
                offset += bytes.Length;
            }

            return data;
        }

        private static byte[] FromHex(string hex)
        {
            hex = hex.Replace("0x", "");
            var raw = new byte[hex.Length / 2];
            for (var i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
    }
}
