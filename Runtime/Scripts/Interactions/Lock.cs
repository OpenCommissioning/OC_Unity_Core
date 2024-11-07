using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using OC.Communication;
using OC.Components;

namespace OC.Interactions
{
    [AddComponentMenu("Open Commissioning/Interactions/Lock")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class Lock : MonoComponent, IDevice, IControlOverridable, ICustomInspector, IInteractable
    {
        public Link Link => _link;
        public IProperty<bool> Override => _override;
        public IProperty<bool> LockSignal => _lock;
        public IPropertyReadOnly<bool> Closed => _closed;
        public IPropertyReadOnly<bool> Locked => _locked;

        [SerializeField]
        protected Property<bool> _override = new (false);
        [SerializeField]
        private Property<bool> _lock = new (false);
        [SerializeField] 
        private Property<bool> _closed = new(false);
        [SerializeField]
        private Property<bool> _locked = new (false);

        [SerializeField]
        private List<Door> _doors = new ();
        [SerializeField]
        private List<Button> _buttons = new ();

        public UnityEvent<bool> OnLockChanged;
        public UnityEvent<bool> OnClosedChanged;
        public UnityEvent<bool> OnLockedChanged;
        
        [SerializeField]
        protected Link _link;
        private ConnectorDataByte _connector;
        
        private bool _lastClosed;
        private bool _lastLock;
        private bool _isValid;

        private void OnEnable()
        {
            _lock.ValueChanged += LockCallback;
            _closed.ValueChanged += ClosedCallback;
            _locked.ValueChanged += LockedCallback;
        }
        
        private void OnDisable()
        {
            _lock.ValueChanged -= LockCallback;
            _closed.ValueChanged -= ClosedCallback;
            _locked.ValueChanged -= LockedCallback;
        }
        
        protected void Start()
        {
            _link.Initialize(this);
            _connector = new ConnectorDataByte(Link);
        }
        
        protected void Reset()
        {
            _link = new Link(this, "FB_Lock");
        }

        private void OnValidate()
        {
            _lock.OnValidate();
            
            if (_buttons.Count > 8)
            {
                Logging.Logger.Log(LogType.Warning, "Buttons max count is 8", this);
                _buttons.RemoveRange(8, _buttons.Count - 8);
            }
        }

        private void LockCallback(bool value)
        {
            foreach (var door in _doors)
            {
                door.Lock.Value = _lock.Value;
            }
            
            OnLockChanged?.Invoke(value);
        }

        private void ClosedCallback(bool value)
        {
            OnClosedChanged?.Invoke(value);
        }
        
        private void LockedCallback(bool value)
        {
            OnLockedChanged?.Invoke(value);
        }

        private void LateUpdate()
        {
            if (!_override && _link.IsConnected.Value) _lock.Value = _connector.Control.GetBit(0);
            UpdateButtons();

            if (_doors.Count > 0)
            {
                _closed.Value = _doors.All(door => door.Closed.Value);
                _locked.Value = _doors.All(door => door.Locked.Value);
            }

            _connector.Status.SetBit(0, _closed);
            _connector.Status.SetBit(1, _locked);
        }

        private void UpdateButtons()
        {
            for (var i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Connector.Control.SetBit(0, _connector.ControlData.GetBit(i));
                _connector.StatusData.SetBit(i, _buttons[i].Connector.Status.GetBit(0));
            }
        }
    }
}
