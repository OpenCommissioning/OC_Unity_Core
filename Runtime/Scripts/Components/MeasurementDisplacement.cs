using UnityEngine;
using UnityEngine.Events;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Sensor/Measurement Displacement")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class MeasurementDisplacement : MonoBehaviour, IMeasurement<float>
    {
        public IPropertyReadOnly<float> Value => _value;

        [Header("Settings")]
        [SerializeField]
        private GameObject _reference;
        [SerializeField]
        private GameObject _target;        
        [SerializeField]
        private Vector2 _displacementLimit;
        [SerializeField] 
        private float _factor = 1;

        [Header("State")]
        [SerializeField]
        protected Property<float> _value = new (0);

        public UnityEvent<float> OnDisplacementChange;
        public UnityEvent<float> OnDisplacementTChange;

        private float _initMagnitude;
        private float _lastValue;
        private float _displacementT;

        private void Start()
        {
            _initMagnitude = Vector3.Distance(_reference.transform.position, _target.transform.position);
            _value.OnValueChanged += OnValueChanged;
        }

        private void OnValueChanged(float value)
        {
            if (value > 0)
            {
                _displacementT = Mathf.InverseLerp(0, _displacementLimit.y, value);
            }
            else
            {
                _displacementT = -Mathf.InverseLerp(0, _displacementLimit.x, value);
            }                

            OnDisplacementChange?.Invoke(value);
            OnDisplacementTChange?.Invoke(_displacementT);
        }

        private void Update()
        {
            CheckDisplacement();
        }

        private void CheckDisplacement()
        {
            _value.Value = CalcDisplacement() * _factor;
        }

        private float CalcDisplacement()
        {
            var value = Vector3.Distance(_reference.transform.position, _target.transform.position) - _initMagnitude;
            return Mathf.Clamp(value, _displacementLimit.x, _displacementLimit.y);
        }
    }
}
