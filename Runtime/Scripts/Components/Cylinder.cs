using System;
using OC.Communication;
using UnityEngine;
using UnityEngine.Events;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Actor/Cylinder")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class Cylinder : Actor, IDevice, ICustomInspector, IInteractable
    {
        public Link Link => _link ?? CreateLink();

        #region Control
        public IProperty<bool> Minus => _minus;
        public IProperty<bool> Plus => _plus;
        [SerializeField]
        protected Property<bool> _minus = new (false);
        [SerializeField]
        protected Property<bool> _plus = new (false);
        
        #endregion

        #region Status
        
        public IPropertyReadOnly<float> Progress => _progress;
        public IPropertyReadOnly<bool> IsActive => _isActive;
        public IPropertyReadOnly<bool> OnLimitMin => _onLimitMin;
        public IPropertyReadOnly<bool> OnLimitMax => _onLimitMax;
        
        [SerializeField]
        protected Property<float> _progress = new (0);
        [SerializeField]
        protected Property<bool> _isActive = new (false);
        [SerializeField]
        protected Property<bool> _onLimitMin = new (false);
        [SerializeField]
        protected Property<bool> _onLimitMax = new (false);

        #endregion

        #region Settings
        
        public IProperty<Vector2> Limits => _limits;
        public IProperty<CylinderType> Type => _type;
        public IProperty<float> TimeToMin => _timeToMin;
        public IProperty<float> TimeToMax => _timeToMax;

        [SerializeField]
        protected Property<Vector2> _limits = new (new Vector2(0, 100));
        [SerializeField]
        protected Property<CylinderType> _type = new (CylinderType.DoubleActing);
        [SerializeField]
        protected Property<float> _timeToMin = new (0.5f);
        [SerializeField]
        protected Property<float> _timeToMax = new (0.5f);
        [SerializeField]
        private AnimationCurve _profile = AnimationCurve.Linear(0, 0, 1, 1);

        #endregion

        #region Events

        public UnityEvent<bool> OnActiveChanged;
        public UnityEvent<bool> OnLimitMinEvent;
        public UnityEvent<bool> OnLimitMaxEvent;

        #endregion

        public bool JogMinus
        {
            set => _minus.Value = value;
            get => _minus;
        }
        
        public bool JogPlus
        {
            set => _plus.Value = value;
            get => _plus;
        }

        [SerializeField]
        protected Link _link;
        [SerializeField]
        private Connector _connector;

        private void Start()
        {
            _link = Link;
            _link.Initialize(this);
            _connector = new Connector(_link);
            _progress.OnValueChanged += OnProgressChanged;
            _isActive.OnValueChanged += value => OnActiveChanged?.Invoke(value);
            _onLimitMin.OnValueChanged += value => OnLimitMinEvent?.Invoke(value);
            _onLimitMax.OnValueChanged += value => OnLimitMaxEvent?.Invoke(value);
            OnProgressChanged(_progress.Value);
        }

        private void OnValidate()
        {
            _minus.OnValidate();
            _plus.OnValidate();
        }

        private void Reset()
        {
            _link = CreateLink();
        }

        private void FixedUpdate()
        {
            if (_link.IsActive) GetLinkData();
            Operation(Time.fixedDeltaTime);
            SetLinkData();
        }

        private void GetLinkData()
        {
            _minus.Value = _connector.Control.GetBit(0);
            _plus.Value = _connector.Control.GetBit(1);
        }

        private void SetLinkData()
        {
            _connector.Status.SetBit(0, _onLimitMin);
            _connector.Status.SetBit(1, _onLimitMax);
        }

        private void Operation(float deltaTime)
        {
            switch (_type.Value)
            {
                case CylinderType.DoubleActing:
                    if (_minus.Value ^ _plus.Value) IntegrateProgress(deltaTime, _plus.Value ? _timeToMax : -_timeToMin);
                    break;
                case CylinderType.SingleActingNegative:
                    IntegrateProgress(deltaTime, _plus.Value ? _timeToMax : -_timeToMin);
                    break;
                case CylinderType.SingleActingPositive:
                    IntegrateProgress(deltaTime, _minus.Value ? -_timeToMin : _timeToMax);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public enum CylinderType
        {
            DoubleActing,
            SingleActingPositive,
            SingleActingNegative
        }

        public void SetProgress(float value) =>  _progress.Value = Mathf.Clamp01(value);

        private void IntegrateProgress(float deltaTime, float duration)
        {
            var progress = _progress.Value + deltaTime / duration;
            _progress.Value = Mathf.Clamp01(progress);
        }
        
        private void OnProgressChanged(float value)
        {
            _target.Value = Mathf.Lerp(_limits.Value.x, _limits.Value.y, _profile.Evaluate(_progress));
            _isActive.Value = Math.FastApproximately(_target, _value, 1e-3f);
            _value.Value = _target.Value;
            _onLimitMin.Value = Math.FastApproximately(_value, _limits.Value.x, 1e-3f);
            _onLimitMax.Value = Math.FastApproximately(_value, _limits.Value.y, 1e-3f);
            SetLinkData();
        }

        private Link CreateLink()
        {
            return new Link(this, "FB_Cylinder");
        }
    }
}

