using System;
using System.Globalization;
using OC.MaterialFlow;
using UnityEngine;
using UnityEngine.Events;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Sensor/Payload Data Reader")]
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class DataReader : Detector, IMeasurement<float>, IInteractable
    {
        public IProperty<string> TargetData => _targetData;
        public IPropertyReadOnly<string> RawData => _rawData;
        public IPropertyReadOnly<float> Value => _value;

        [SerializeField]
        protected Property<string> _targetData = new ("");
        [SerializeField]
        protected Property<string> _rawData = new ("");
        [SerializeField]
        protected Property<float> _value = new ();
        [SerializeField]
        private string _key;
        [SerializeField] 
        private bool _autoRead;
        [SerializeField] 
        private bool _cyclic;
        
        public UnityEvent<string> OnDataChangedEvent;
        public UnityEvent<float> OnValueChangedEvent;

        private bool _isDataAvailable;
        private PayloadBase _payloadBase;
        private BoxCollider _collider;
        private Rigidbody _rigidbody;

        private new void OnEnable()
        {
            base.OnEnable();
            _rawData.OnValueChanged += OnDataChanged;
            _value.OnValueChanged += OnValueChanged;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            _rawData.OnValueChanged -= OnDataChanged;
            _value.OnValueChanged -= OnValueChanged;
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if (_cyclic)
            {
                if (_collision.Value)
                {
                    Read();
                }
                else
                {
                    if (_isDataAvailable) Clear();
                }
            }
        }

        protected override void OnPayloadEnterAction(PayloadBase payloadBase)
        {
            base.OnPayloadEnterAction(payloadBase);
            _payloadBase = payloadBase;
            if (_autoRead) Read();
        }
        
        protected override void OnPayloadExitAction(PayloadBase payloadBase)
        {
            base.OnPayloadExitAction(payloadBase);
            _payloadBase = null;
        }

        public void Read()
        {
            try
            {
                if (_payloadBase == null) throw new Exception("Payload Base is null!");
                if (!_payloadBase.TryGetComponent(out PayloadData payloadData)) throw new Exception("Payload Data isn't found!");
                payloadData.GetValue(_key, out var result);
                _rawData.Value = result;
                
                if (float.TryParse(result, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                {
                    _value.Value = value;
                }

                _isDataAvailable = true;
            }
            catch (Exception exception)
            {
                Logging.Logger.LogError(exception.Message, this);
            }
        }

        public void Write()
        {
            try
            {
                if (_payloadBase == null) throw new Exception("Payload Base is null!");
                if (!_payloadBase.TryGetComponent(out PayloadData payloadata)) throw new Exception("Payload Data isn't found!");
                payloadata.SetValue(_key, _targetData.Value);
            }
            catch (Exception exception)
            {
                Logging.Logger.LogError(exception.Message, this);
            }
        }

        public void Clear()
        {
            _payloadBase = null;
            _rawData.Value = "";
            _value.Value = 0;
            _isDataAvailable = false;
        }
        
        private void OnDataChanged(string value)
        {
            OnDataChangedEvent?.Invoke(value);
        }
        
        private void OnValueChanged(float value)
        {
            OnValueChangedEvent?.Invoke(value);
        }
        
        private void Initialize()
        {
            if (_collider == null) _collider = GetComponent<BoxCollider>();
            if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();
            
            _collider.isTrigger = true;
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }
    }
}
