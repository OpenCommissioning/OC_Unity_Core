using UnityEngine;

namespace OC.Communication
{
    public class Hierarchy : MonoBehaviour
    {
        public string Name => GetName();
        public bool IsNameSampler => _isNameSampler;
        public Hierarchy Parent => _parent;
        
        [SerializeField]
        private string _name;
        [SerializeField]
        private bool _isNameSampler;
        [SerializeField]
        private Hierarchy _parent;

        private string GetName()
        {
            return string.IsNullOrEmpty(_name) ? transform.name : _name;
        }

        public Transform GetParent()
        {
            return _parent != null ? _parent.transform : transform.parent;
        }
    }
}
