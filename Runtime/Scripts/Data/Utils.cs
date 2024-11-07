using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OC.Data
{
    public static class Utils
    {
        public static List<MetadataAsset> GetMetadataAssets(Component component)
        {
            var components = component.GetComponents<IComponentMetadata>().ToList();
            return components.Select(item => item.GetAsset()).ToList();
        }
        
        public static string GetStreamingAssetsPath()
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            return Application.streamingAssetsPath;
        }
    }
}