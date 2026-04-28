using NUnit.Framework;
using OC.Data;
using UnityEngine;

namespace OC.Tests.Editor.Data
{
    public class TestTagDirectoryManager
    {
        private ProductDataDirectoryManager _productDataDirectoryManager;

        [SetUp]
        public void SetUp()
        {
            var gameObject = new GameObject();
            _productDataDirectoryManager = gameObject.AddComponent<ProductDataDirectoryManager>();
            _productDataDirectoryManager.ProductDataDirectories.Add(new ProductDataDirectory("DataPath1", "D:/Test/DataPath1"));
            _productDataDirectoryManager.ProductDataDirectories.Add(new ProductDataDirectory("DataPath2", "D:/Test/DataPath2"));
            _productDataDirectoryManager.ProductDataDirectories.Add(new ProductDataDirectory("DataPath3", "D:/Test/DataPath3"));
        }
        
        [Test]
        [TestCase(-1, false)]
        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, false)]
        public void IsFolderIdValid(int folderId, bool expected)
        {
            Assert.That(expected == _productDataDirectoryManager.Contains(folderId), $"Pool Prefabs Count: {_productDataDirectoryManager.ProductDataDirectories.Count}, Input FolderId {folderId}");
        }
    }
}