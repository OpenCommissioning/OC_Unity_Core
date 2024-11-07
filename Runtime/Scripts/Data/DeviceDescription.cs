using System;
using UnityEngine;

namespace OC.Data
{
    [Serializable]
    public struct DeviceDescription
    {
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        
        public MetadataAsset MetadataAsset
        {
            get => _metadataAsset;
            set => _metadataAsset = value;
        }

        [SerializeField]
        private string _name;
        [SerializeField] 
        private MetadataAsset _metadataAsset;

        public DeviceDescription(IDeviceMetadata device)
        {
            _name = device.Component.transform.GetPath();
            _metadataAsset = device.GetAsset();
        }

        public void Deserialize()
        {
            try
            {
                var gameObject = GameObject.Find(Name);
                if (gameObject == null) throw new Exception($"Device not found in project! (at: {Name})");
                if (gameObject.TryGetComponent(out IDeviceMetadata metadata))
                {
                    metadata.SetAsset(_metadataAsset);
                }
                else
                {
                    throw new Exception($"IDeviceData not found gameObject! (at: {Name})");
                }

            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, exception.Message);
                throw;
            }
        }
    }
}
