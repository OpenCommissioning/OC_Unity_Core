using UnityEngine;

namespace OC.Components
{
    [ExecuteInEditMode]
    public class CrankSlider : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private bool _showGizmos;

        [Header("References")]
        [SerializeField]
        private Transform _crankPin;
        [SerializeField]
        private Transform _slider;

        private bool _isInitialized;
        private float _linkRadius;
        private bool _isRight;
        private Vector3 _slideStart;
        private Vector3 _slideEnd;
        private Vector3 _pinLocalPosition;
        private Vector3 _sliderLocalPosition;
        private Vector2[] _intersections;

        private bool _isReferenced;

        private void Start()
        {
            Initialize();
            _isReferenced = _slider is not null && _crankPin is not null;
        }

        private void Update()
        {
            if (!_isReferenced) return;
            SetSliderPosition();
        }

        private void OnValidate()
        {
            _isInitialized = false;
        }

        private void Initialize()
        {
            if (!_isReferenced) return;
            if (_isInitialized) return;

            _isInitialized = true;

            _pinLocalPosition = transform.InverseTransformPoint(_crankPin.position);
            _sliderLocalPosition = transform.InverseTransformPoint(_slider.position);
            _linkRadius = Vector2.Distance(_pinLocalPosition, _sliderLocalPosition);
            _isRight = (_sliderLocalPosition.x - _pinLocalPosition.x >= 0.0);

            _slideStart = transform.InverseTransformPoint(_slider.position + _slider.forward * 5);
            _slideEnd = transform.InverseTransformPoint(_slider.position - _slider.forward * 5);
        }

        private void SetSliderPosition()
        {
            _slider.transform.localEulerAngles = Math.PlaneXYAngle(_slider.transform.localEulerAngles);
            _pinLocalPosition = transform.InverseTransformPoint(_crankPin.position);

            _intersections = Math.GetIntersections(_slideStart, _slideEnd, _pinLocalPosition, _linkRadius);

            if (_intersections == null || _intersections.Length == 0) return;

            if (_intersections.Length == 1)
            {
                _sliderLocalPosition = _intersections[0];
            }

            if (_intersections.Length == 2)
            {
                _sliderLocalPosition = _isRight ? _intersections[1] : _intersections[0];
            }

            _slider.transform.position = transform.TransformPoint(_sliderLocalPosition);
        }

        private void OnDrawGizmos()
        {
            if (!_showGizmos) return;

            Initialize();
#if UNITY_EDITOR
            UnityEditor.Handles.matrix = transform.localToWorldMatrix;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(_pinLocalPosition, Vector3.back, _linkRadius);
#endif

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.red;            
            Gizmos.DrawSphere(_pinLocalPosition, 0.02f);
            Gizmos.DrawSphere(_sliderLocalPosition, 0.02f);
            Gizmos.DrawSphere(_sliderLocalPosition, 0.02f);
        }
    }
}


