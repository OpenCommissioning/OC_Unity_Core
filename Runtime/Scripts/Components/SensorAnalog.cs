using OC.Communication;
using UnityEngine;
using UnityEngine.Events;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Sensor/Sensor Analog")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class SensorAnalog : MonoComponent, IDevice, ICustomInspector, IInteractable
    {
        public Link Link => _link;
        public IProperty<bool> Override => _override;
        public IProperty<float> Value => _value;
        
        [SerializeField]
        protected Property<bool> _override = new (false);
        [SerializeField]
        protected Property<float> _value = new (0f);

        [SerializeField]
        private Component _valueSource;
        [SerializeField] 
        private Property<float> _factor = new(1f);

        public UnityEvent<float> OnValueChangedEvent;

        [SerializeField]
        private Link _link;

        private bool _isDeviceValid;
        private ConnectorDataFloat _connector;
        private IMeasurement<float> _measurementDevice;

        private void OnEnable()
        {
            _value.OnValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            _value.OnValueChanged -= OnValueChanged;
        }
        
        private void Start()
        { 
            _link.Initialize(this);
            _connector = new ConnectorDataFloat(_link);
            GetValueSource();
            OnValueChanged(_value.Value);
        }

        private void Reset()
        {
            _link = new Link(this, "FB_SensorAnalog");
        }

        public void OnValidate()
        {
            _value.OnValidate();
        }

        public void SetValue(float value)
        {
            OnDeviceValueChanged(value);
        }

        private void GetValueSource()
        {
            _isDeviceValid = false;
            if (_valueSource == null) return;
            if (_valueSource.TryGetComponent(out _measurementDevice))
            {
                _isDeviceValid = true;
                OnDeviceValueChanged(_measurementDevice.Value.Value);
                _measurementDevice.Value.OnValueChanged += OnDeviceValueChanged;
            }
           
            if (!_isDeviceValid) Logging.Logger.Log(LogType.Error, "Device reference isn't valid! IMeasurementDevice is required!", this);
        }

        private void OnDeviceValueChanged(float value)
        {
            if (_override) return;
            _value.Value = value * _factor.Value;
        }

        private void OnValueChanged(float value)
        {
            _connector.StatusData = _value.Value;
            OnValueChangedEvent?.Invoke(_value.Value);
        }
    }
}
