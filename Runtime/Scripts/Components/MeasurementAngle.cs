using UnityEngine;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Sensor/Measurement Angle")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class MeasurementAngle : MonoBehaviour, IMeasurement<float>
    {
        public IPropertyReadOnly<float> Value => _value;

        [Header("Settings")]
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private float _offset;
        [SerializeField]
        private MeasurementDirection _direction;

        [Header("State")]
        [SerializeField]
        protected Property<float> _value = new (0);

        private void Update()
        {
            _value.Value = GetAngle();
        }

        private float GetAngle()
        {
            var eulerAngles = _target.transform.eulerAngles;
            return _direction switch
            {
                MeasurementDirection.X_Positiv => eulerAngles.x + _offset,
                MeasurementDirection.Y_Positiv => eulerAngles.y + _offset,
                MeasurementDirection.Z_Positiv => eulerAngles.z + _offset,
                MeasurementDirection.X_Negativ => -eulerAngles.x + _offset,
                MeasurementDirection.Y_Negativ => -eulerAngles.y + _offset,
                MeasurementDirection.Z_Negativ => -eulerAngles.z + _offset,
                _ => 0,
            };
        }

        private enum MeasurementDirection
        {
            // ReSharper disable once InconsistentNaming
            X_Positiv,
            // ReSharper disable once InconsistentNaming
            Y_Positiv,
            // ReSharper disable once InconsistentNaming
            Z_Positiv,
            // ReSharper disable once InconsistentNaming
            X_Negativ,
            // ReSharper disable once InconsistentNaming
            Y_Negativ,
            // ReSharper disable once InconsistentNaming
            Z_Negativ
        }
    }
}
