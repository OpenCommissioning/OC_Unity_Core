using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OC.Editor
{
    public static class LayerManager
    {
        public static void ApplyDefaultLayers()
        {
            var defaultLayers = System.Enum.GetValues(typeof(DefaultLayers)) as DefaultLayers[];
            if (defaultLayers == null || defaultLayers.Length < 1)
            {
                Logging.Logger.LogWarning("Default layer configuration can't be found!");
                return;
            }
            
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagManagerLayers = tagManager.FindProperty("layers");

            foreach (var defaultLayer in defaultLayers)
            {
                var layerProperty = tagManagerLayers.GetArrayElementAtIndex((int)defaultLayer);
                layerProperty.stringValue = defaultLayer.ToString();
                tagManager.ApplyModifiedProperties();
            }

            var layers = GetAllLayers();
            
            foreach(var d1 in layers)
            {
                foreach (var d2 in layers)
                {
                    Physics.IgnoreLayerCollision(d1.Value, d2.Value, true);
                }
            }
            
            Physics.IgnoreLayerCollision(layers["Default"], layers["Default"], false);
            Physics.IgnoreLayerCollision(layers["Default"], layers[DefaultLayers.Transport.ToString()], false);
            Physics.IgnoreLayerCollision(layers["Default"], layers[DefaultLayers.Interactions.ToString()], false);
            Physics.IgnoreLayerCollision(layers[DefaultLayers.Stopper.ToString()], layers[DefaultLayers.Stopper.ToString()], false);
            Physics.IgnoreLayerCollision(layers[DefaultLayers.Reader.ToString()], layers[DefaultLayers.Reader.ToString()], false);

            Logging.Logger.Log("Settings for Layers and Collision Matrix are set to default Open Commissioning configuration!");
        }

        private static Dictionary<string, int> GetAllLayers()
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var layers = tagManager.FindProperty("layers");
            var layerSize = layers.arraySize;

            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            for (int i = 0; i < layerSize; i++)
            {
                SerializedProperty element = layers.GetArrayElementAtIndex(i);
                string layerName = element.stringValue;

                if (!string.IsNullOrEmpty(layerName))
                {
                    dictionary.Add(layerName, i);
                }
            }

            return dictionary;
        } 
    }
}
