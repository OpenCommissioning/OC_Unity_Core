using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OC
{
    [SelectionBase]
    [ExecuteAlways]
    public class SnappingObject : MonoBehaviour
    {
        public List<SnappingPoint> Points => _points;
        
        [Header("References")]
        [SerializeField]
        private List<SnappingPoint> _points;
        
        [Header("Gizmos")]
        [SerializeField]
        private bool _drawGizmos = true;
        [SerializeField]
        private float _gizmosRadius = 0.05f;
        
        private void OnValidate()
        {
            Refresh();
        }

        private void OnTransformChildrenChanged()
        {
            Refresh();
        }

        private void Refresh()
        {
            _points = GetComponentsInChildren<SnappingPoint>().ToList();
        }

        private void OnDrawGizmos()
        {
            foreach (var point in Points)
            {
                point.DrawGizmos = _drawGizmos;
            }
        }
    }
}
