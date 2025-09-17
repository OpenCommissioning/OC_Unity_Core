using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace OC.SnapingTool
{
    [ExecuteInEditMode]
    public class SnapPoint : MonoBehaviour
    {
        public int GroupId => _groupId;
        public GameObject Parent => _parent;
        public Type SnapType  => _type;  
        [SerializeField]
        private int _groupId;

        [SerializeField]
        private GameObject _parent;

        [FormerlySerializedAs("_snapType")]
        [SerializeField]
        private Type _type = Type.All;

        [SerializeField]
        private bool _drawGizmos = true;

        [SerializeField]
        private Color _gizmosColor = Color.blue;

        [SerializeField]
        private float _gizmosRadius = 0.05f;

        private Matrix4x4 _offset;

        private void SetOffset()
        {
            var parent = _parent.transform.GetMatrix();
            var point = transform.GetMatrix();
            _offset = parent.inverse * point;
        }

        public void SetGlobalMatrix(Matrix4x4 matrix)
        {
            var result = matrix * _offset.inverse;
            _parent.transform.SetMatrix(result);
        }

        private void OnValidate()
        {
            //if (_parent == null) _parent = gameObject.transform.parent != null ? gameObject.transform.parent.gameObject : gameObject;
            SetOffset();
        }

        private void Reset()
        {
            _parent = gameObject.transform.parent != null ? gameObject.transform.parent.gameObject : gameObject;
        }

        private void Update()
        {
            SetOffset();
        }

        private void OnDrawGizmos()
        {
            if (!_drawGizmos) return;
            Gizmos.color = _gizmosColor;
            Gizmos.DrawSphere(transform.position, _gizmosRadius);
        }

        public void SnapToPoint(Transform point)
        {
            var parentOffset = _parent.transform.position - transform.position;
            _parent.transform.position = point.position + parentOffset;
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