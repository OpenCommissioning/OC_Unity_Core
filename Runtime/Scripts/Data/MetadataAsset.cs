using System;
using System.Collections.Generic;
using UnityEngine;

namespace OC.Data
{
    [Serializable]
    public class MetadataAsset
    {
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        
        public List<DictionaryItem> Data
        {
            get => _data;
            set => _data = value;
        }

        [SerializeField]
        private string _name;
        [SerializeField] 
        private List<DictionaryItem> _data = new ();

        public MetadataAsset(){}
        
        public MetadataAsset(string name)
        {
            _name = name;
        }
    }
}