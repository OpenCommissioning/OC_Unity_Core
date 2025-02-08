using OC.Communication;
using UnityEngine;
using UnityEngine.Events;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Signal Binary")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class SignalBinary : MonoComponent, IDevice, IMeasurement<bool>, IControlOverridable, ICustomInspector, IInteractable
    {
        public Link Link => _link;
        public IProperty<bool> Override => _override;
        public IPropertyReadOnly<bool> Value => _value;
        public IProperty<bool> Signal => _signal;
        
        public UnityEvent<bool> OnValueChangedEvent;
        
        [SerializeField]
        protected Property<bool> _override = new (false);
        [SerializeField]
        protected Property<bool> _value = new (false);
        [SerializeField]
        protected Property<bool> _signal = new (false);
        [SerializeField]
        private Component _device;
        [SerializeField]
        private Link _link;

        private Connector _connector;
        private bool _isMeasurementDeviceValid;
        private IMeasurement<bool> _measurementDevice;
        
        private void OnEnable()
        {
            _signal.OnValueChanged += OnSignalChanged;
            _value.OnValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            _signal.OnValueChanged -= OnSignalChanged;
            _value.OnValueChanged -= OnValueChanged;
        }
        
        private void Start()
        {
            _link.Initialize(this);
            _connector = new Connector(_link);
            GetMeasurementDevice();
            OnValueChanged(_value.Value);
        }

        private void Reset()
        {
            _link = new Link(this, "FB_SensorBinary");
        }
        
        private void OnSignalChanged(bool value)
        {
            _value.Value = value;
        }
        
        private void OnValueChanged(bool value)
        {
            _connector.Status.SetBit(0, value);
            OnValueChangedEvent?.Invoke(value);
        }
        
        private void GetMeasurementDevice()
        {
            _isMeasurementDeviceValid = false;
            
            if (_device == null) return;
            if (_device is IMeasurement<bool> device)
            {
                _isMeasurementDeviceValid = true;
                _measurementDevice = device;
                _measurementDevice.Value.OnValueChanged += OnDeviceValueChanged;
                OnDeviceValueChanged(_measurementDevice.Value.Value);
            }
           
            if (!_isMeasurementDeviceValid) Logging.Logger.Log(LogType.Error, "Device reference isn't valid! IMeasurementDevice is required!", this);
        }
        
        private void OnDeviceValueChanged(bool value)
        {
            if (_override) return;
            _signal.Value = value;
        }
    }
}