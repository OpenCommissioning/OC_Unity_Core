using System;
using System.Collections.Generic;
using OC.Components;
using OC.Data;
using UnityEngine;

namespace OC.MaterialFlow
{
    [RequireComponent(typeof(Payload))]
    [DisallowMultipleComponent]
    public class PayloadData : MonoComponent, IComponentMetadata
    {
        [SerializeField]
        private List<DictionaryItem> _data = new ();

        public void GetValue(string key, out string value)
        {
            try
            {
                var metadata = _data.Find(item => item.Key == key);
                if (metadata is null) throw new Exception("Key can't be found!");
                value = metadata.Value;
            }
            catch (Exception exception)
            {
                Logging.Logger.LogError(exception.Message, this);
                value = "";
            }
        }
        
        public void SetValue(string key, string value)
        {
            try
            {
                var metadata = _data.Find(item => item.Key == key);
                if (metadata is null) throw new Exception("Key can't be found!");
                metadata.Value = value;

            }
            catch (Exception exception)
            {
                Logging.Logger.LogError(exception.Message, this);
            }
        }

        public MetadataAsset GetAsset()
        {
            var asset = new MetadataAsset("PayloadData");
            foreach (var dataItem in _data)
            {
                asset.Data.Add(dataItem);
            }
            return asset;
        }

        public void SetAsset(MetadataAsset asset)
        {
            _data.Clear();
            foreach (var metadataItem in asset.Data)
            {
                _data.Add(new DictionaryItem(metadataItem.Key, metadataItem.Value));
            }
        }
    }
}