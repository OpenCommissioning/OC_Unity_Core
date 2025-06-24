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
    public class Lock : MonoComponent, IDevice, ICustomInspector, IInteractable
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
        private List<Device> _buttons = new ();

        public UnityEvent<bool> OnLockChanged;
        public UnityEvent<bool> OnClosedChanged;
        public UnityEvent<bool> OnLockedChanged;
        
        [SerializeField]
        protected LinkDataByte _link;

        private void OnEnable()
        {
            _lock.OnValueChanged += LockCallback;
            _closed.OnValueChanged += ClosedCallback;
            _locked.OnValueChanged += LockedCallback;
        }
        
        private void OnDisable()
        {
            _lock.OnValueChanged -= LockCallback;
            _closed.OnValueChanged -= ClosedCallback;
            _locked.OnValueChanged -= LockedCallback;
        }
        
        protected void Start()
        {
            _link.Initialize(this);
        }
        
        protected void Reset()
        {
            _link = new LinkDataByte
            {
                Type = "FB_Lock"
            };
        }

        private void OnValidate()
        {
            _lock.OnValidate();
            
            var index = 0;
            foreach (var item in _buttons)
            {
                for (var j = 0; j < item.AllocatedBitLength; j++)
                {
                    index++;
                }
            }
            
            if (index > 8)
            {
                Logging.Logger.Log(LogType.Error, "Interface length is exceeded! Max length is 8 bit", this);
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
            if (_link.IsActive) _lock.Value = _link.Control.GetBit(0);
            UpdateButtons();

            if (_doors.Count > 0)
            {
                _closed.Value = _doors.All(door => door.Closed.Value);
                _locked.Value = _doors.All(door => door.Locked.Value);
            }

            _link.Status.SetBit(0, _closed);
            _link.Status.SetBit(1, _locked);
        }

        private void UpdateButtons()
        {
            var index = 0;
            foreach (var item in _buttons)
            {
                for (var j = 0; j < item.AllocatedBitLength; j++)
                {
                    _link.StatusData.SetBit(index, item.Link.Status.GetBit(j));
                    item.Link.Control.SetBit(j,_link.ControlData.GetBit(index));
                    index++;
                }
            }
        }
    }
}
