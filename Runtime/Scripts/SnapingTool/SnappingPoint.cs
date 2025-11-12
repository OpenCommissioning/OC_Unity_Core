using System;
using UnityEngine;

namespace OC
{
    [ExecuteInEditMode]
    public class SnappingPoint : MonoBehaviour
    {
        public int GroupId => _groupId;
        public GameObject Parent => _parent;
        public Type SnapType  => _type;

        public bool DrawGizmos
        {
            get => _drawGizmos;
            set => _drawGizmos = value;
        }

        public Color GizmosColor
        {
            get => _gizmosColor;
            set => _gizmosColor = value;
        }
        
        [SerializeField]
        private int _groupId;
        [SerializeField]
        private GameObject _parent;
        [SerializeField]
        private Type _type = Type.All;
        [SerializeField]
        private bool _drawGizmos = true;
        [SerializeField]
        private Color _gizmosColor = Color.blue;
        [SerializeField]
        private float _gizmosRadius = 0.05f;

        private void Reset()
        {
            _parent = gameObject.transform.parent != null ? gameObject.transform.parent.gameObject : gameObject;
        }

        private void OnDrawGizmos()
        {
            if (!_drawGizmos) return;
            Gizmos.color = _gizmosColor;
            Gizmos.DrawSphere(transform.position, _gizmosRadius);
        }

        [Flags]
        public enum Type
        {
            None = 0,
            Plug = 1,
            Slot = 2,
            All = ~0 
        }
    }
}