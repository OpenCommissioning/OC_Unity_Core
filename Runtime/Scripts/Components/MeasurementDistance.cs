using UnityEngine;
using UnityEngine.Events;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Sensor/Measurement Distance")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class MeasurementDistance : MonoBehaviour, IMeasurement<float>, ISensorBeam
    {
        public IProperty<bool> State => _collision;
        public IPropertyReadOnly<float> Value => _value;
        public IPropertyReadOnly<float> Length => _value;

        [SerializeField]
        protected Property<bool> _collision = new (false);
        [SerializeField]
        protected Property<float> _value = new (0);

        [Header("Settings")]
        [SerializeField] 
        private float _length = 1;
        [SerializeField] 
        private LayerMask _layerMask;

        [SerializeField]
        private bool _showGizmos;

        public UnityEvent<float> OnValueChangedEvent;
        
        private readonly RaycastHit[] _raycastHit = new RaycastHit[5];
        private float _distance;

        private void OnEnable()
        {
            _collision.ValueChanged += OnCollisionChanged;
            _value.ValueChanged += OnValueChanged;
        }
        
        private void OnDisable()
        {
            _collision.ValueChanged -= OnCollisionChanged;
            _value.ValueChanged -= OnValueChanged;
        }

        private void Start()
        {
            _value.Value = _length;
        }
        
        private void FixedUpdate()
        {
            var hits = Physics.RaycastNonAlloc(transform.position, transform.forward, _raycastHit, _length, _layerMask);
            if (hits > 0)
            {
                _collision.Value = true;
                _distance = _length;
                for (var i = 0; i < hits; i++)
                {
                    if (_raycastHit[i].distance < _distance)
                    {
                        _distance = _raycastHit[i].distance;
                    }
                }

                _value.Value = _distance;
            }
            else
            {
                _collision.Value = false;
                _value.Value = _length;
            }
        }

        private void OnValueChanged(float value)
        {
            OnValueChangedEvent?.Invoke(value);
        }

        private void OnCollisionChanged(bool value)
        {
            if (!value) _value.Value = _length;
        }

        private void OnDrawGizmos()
        {
            if (!_showGizmos) return;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Vector3.zero, 0.005f);
            Gizmos.DrawSphere(Vector3.forward * _length, 0.005f);
            Gizmos.DrawLine(Vector3.zero, Vector3.forward * _length);
        }
    }
}
