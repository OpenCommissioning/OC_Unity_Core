using System;
using UnityEngine;

namespace OC.Data
{
    [Serializable]
    public class ProductDataDirectory
    {
        private const string STREAMING_ASSETS_PREFIX = "streamingassets:";
        
        public string Name;
        public string Path;

        public ProductDataDirectory(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public string GetValidPath()
        {
            if (!Path.StartsWith(STREAMING_ASSETS_PREFIX, StringComparison.OrdinalIgnoreCase)) return Path;
            var relativePath = Path.Substring(STREAMING_ASSETS_PREFIX.Length).TrimStart('/','\\');
            return System.IO.Path.Combine(Application.streamingAssetsPath, relativePath);
        }
    }
}
