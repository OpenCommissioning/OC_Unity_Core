using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Sensor/Measurement Displacement Force")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class MeasurementDisplacementForce : MonoBehaviour, IMeasurement<float>
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
        private float _springConstant = 5000;
        [SerializeField]
        private Vector2 _forceLimit;

        [Header("State")]
        [SerializeField]
        private float _displacement;
        [SerializeField]
        protected Property<float> _value = new (0);

        [Foldout("Events")]
        public UnityEvent<float> OnDisplacementChange;
        [Foldout("Events")]
        public UnityEvent<float> OnDisplacementTChange;

        private float _initMagnitude;
        private float _lastValue;
        private float _displacementT;

        private void Start()
        {
            _initMagnitude = Vector3.Distance(_reference.transform.position, _target.transform.position);
        }

        private void Update()
        {
            CheckDisplacement();
        }

        private void CheckDisplacement()
        {
            _displacement = CalcDisplacement();

            if (!(Mathf.Abs(_displacement - _lastValue) > Utils.TOLERANCE)) return;
            _lastValue = _displacement;
            if (_displacement > 0)
            {
                _displacementT = Mathf.InverseLerp(0, _displacementLimit.y, _displacement);
            }
            else
            {
                _displacementT = -Mathf.InverseLerp(0, _displacementLimit.x, _displacement);
            }
            _value.Value = CalcForce(_displacement);
            OnDisplacementChange?.Invoke(_displacement * 1000);
            OnDisplacementTChange?.Invoke(_displacementT);
        }

        private float CalcDisplacement()
        {
            var value = Vector3.Distance(_reference.transform.position, _target.transform.position) - _initMagnitude;
            return Mathf.Clamp(value, _displacementLimit.x, _displacementLimit.y);
        }

        private float CalcForce(float displacement)
        {
            var force = _springConstant * displacement;
            return Mathf.Clamp(force, _forceLimit.x, _forceLimit.y);
        }
    }
}
