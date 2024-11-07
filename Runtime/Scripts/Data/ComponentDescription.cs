using System;
using System.Collections.Generic;
using UnityEngine;

namespace OC.Data
{
    [Serializable]
    public struct ComponentDescription
    {
        public string Path
        {
            get => _path;
            set => _path = value;
        }
        
        public string Type
        {
            get => _type;
            set => _type = value;
        }
        
        public int Index
        {
            get => _index;
            set => _index = value;
        }
        
        public MetadataAsset MetadataAsset
        {
            get => _metadataAsset;
            set => _metadataAsset = value;
        }
        
        [SerializeField]
        private string _path;
        [SerializeField]
        private string _type;
        [SerializeField]
        private int _index;
        [SerializeField] 
        private MetadataAsset _metadataAsset;

        public ComponentDescription(IComponentMetadata componentMetadata)
        {
            _path = componentMetadata.Component.transform.GetPath();
            _type = componentMetadata.Component.GetType().ToString();
            
            var components = new List<IComponentMetadata>();
            componentMetadata.Component.gameObject.GetComponents(components);

            _index = components.IndexOf(componentMetadata);
            _metadataAsset = componentMetadata.GetAsset();
        }

        public void Deserialize()
        {
            try
            {
                var gameObject = GameObject.Find(Path);
                if (gameObject == null) throw new Exception($"Gameobject isn't found! (at: {Path})");
                
                var components = new List<IComponentMetadata>();
                gameObject.GetComponents(components);

                if (!OC.Utils.IsInRange(_index, 0, components.Count-1))
                {
                    throw new Exception($"IComponentMetadata at Index {_index} can't be found!");
                }

                var actualType = components[_index].GetType().ToString();

                if (_type != actualType)
                {
                    throw new Exception($"IComponentMetadata type isn't valid! Expected {_type} but was {actualType}");
                }
                
                components[_index].SetAsset(_metadataAsset);
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, exception.Message);
                throw;
            }
        }
    }
}