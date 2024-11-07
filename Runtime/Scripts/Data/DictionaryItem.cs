using System;
using UnityEngine;

namespace OC.Data
{
    [Serializable]
    public class DictionaryItem
    {
        public string Key
        {
            get => _key;
            set => _key = value;
        }

        public string Value
        {
            get => _value;
            set => _value = value;
        }
            
        [SerializeField]
        private string _key;
        [SerializeField]
        private string _value;

        public DictionaryItem(string key, string value)
        {
            _key = key;
            _value = value;
        }

        public void Set(DictionaryItem other)
        {
            _key = other._key;
            _value = other._value;
        }
    }
}