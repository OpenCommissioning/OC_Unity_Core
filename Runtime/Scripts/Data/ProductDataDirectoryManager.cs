using System.Collections.Generic;
using System.Xml.Linq;
using OC.Project;
using UnityEngine;

namespace OC.Data
{
    public class ProductDataDirectoryManager : MonoBehaviourSingleton<ProductDataDirectoryManager>, IConfigAsset
    {
        public List<ProductDataDirectory> ProductDataDirectories
        {
            get => _productDataDirectories;
            set => _productDataDirectories = value;
        }

        [SerializeField]
        private List<ProductDataDirectory> _productDataDirectories = new();

        public bool Contains(int index)
        {
            return index >= 0 && index < _productDataDirectories.Count;
        }

        public List<ProductDataDirectory> GetValidDataDirectories(List<int> directoryIndexes)
        {
            var result = new List<ProductDataDirectory>();
            foreach (var index in directoryIndexes)
            {
                if (!Contains(index)) continue;
                result.Add(_productDataDirectories[index]);
            }
            return result;
        }
        
        public XElement GetAsset()
        {
            var element = _productDataDirectories.ToXElement<List<ProductDataDirectory>>();
            element.SetAttributeValue("Name", name);
            return element;
        }

        public void SetAsset(XElement xElement)
        {
            _productDataDirectories = xElement.FromXElement<List<ProductDataDirectory>>();
        }
    }
}