
using System.Xml.Linq;
using UnityEngine;

namespace OC.Project
{
    public interface IConfigAsset
    {
        public Component Component { get; }
        public XElement GetAsset();
        public void SetAsset(XElement xElement);
    }
}