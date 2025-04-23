#if UNITY_EDITOR
    using UnityEditor;
#endif
    
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
            if (string.IsNullOrEmpty(_name))
            {
                if (!ClientVariableExtension.IsVariableNameValid(transform.name))
                {
                    var validName = ClientVariableExtension.CorrectVariableName(transform.name);
#if UNITY_EDITOR
                    Debug.LogWarning($"Hierarchy GameObject name {transform.name} is invalid! The name is modified to {validName}", this);
                    transform.name = validName;
                    EditorUtility.SetDirty(this);
#endif
                }
                
                return transform.name;
            }
            else
            {
                if (!ClientVariableExtension.IsVariableNameValid(_name))
                {
                    var validCustomName = ClientVariableExtension.CorrectVariableName(_name);
#if UNITY_EDITOR
                    Debug.LogWarning($"Hierarchy name {_name} is invalid! The name is modified to {validCustomName}");
                    _name = validCustomName;
                    EditorUtility.SetDirty(this);
#endif
                }
                
                return _name;
            }
        }

        public Transform GetParent()
        {
            return _parent != null ? _parent.transform : transform.parent;
        }
    }
}
