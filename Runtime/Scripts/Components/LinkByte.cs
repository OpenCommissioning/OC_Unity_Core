using System;
using NaughtyAttributes;
using OC.Communication;
using UnityEngine;
using UnityEngine.Events;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Links/Link Byte")]
    public class LinkByte : MonoComponent, IDevice
    {
        public Link Link => _link;

        [ReadOnly]
        [SerializeField] 
        private byte _value;

        [Header("Events")]
        public UnityEvent<bool> OnBit0Changed;
        public UnityEvent<bool> OnBit1Changed;
        public UnityEvent<bool> OnBit2Changed;
        public UnityEvent<bool> OnBit3Changed;
        public UnityEvent<bool> OnBit4Changed;
        public UnityEvent<bool> OnBit5Changed;
        public UnityEvent<bool> OnBit6Changed;
        public UnityEvent<bool> OnBit7Changed;

        [SerializeField]
        private LinkDataByte _link;

        private void Start()
        {
            Link.Initialize(this);
        }
        
        private void Reset()
        {
            _link = new LinkDataByte
            {
                Type = "FB_DeviceByte"
            };
        }

        private void Update()
        {
            if (!_link.IsActive) return;
            if (_link.ControlData == _value) return;
            
            _value = _link.ControlData;
            OnBit0Changed.Invoke(_link.ControlData.GetBit(0));
            OnBit1Changed.Invoke(_link.ControlData.GetBit(1));
            OnBit2Changed.Invoke(_link.ControlData.GetBit(2));
            OnBit3Changed.Invoke(_link.ControlData.GetBit(3));
            OnBit4Changed.Invoke(_link.ControlData.GetBit(4));
            OnBit5Changed.Invoke(_link.ControlData.GetBit(5));
            OnBit6Changed.Invoke(_link.ControlData.GetBit(6));
            OnBit7Changed.Invoke(_link.ControlData.GetBit(7));
        }

        public void SetDataBit(int index, bool value)
        {
            if (index is < 0 or > 7) throw new ArgumentOutOfRangeException(nameof(index));
            _link.StatusData.SetBit(index, value);
        }

        public void SetDataBit0(bool value)
        {
            _link.StatusData.SetBit(0, value);
        }
        
        public void SetDataBit1(bool value)
        {
            _link.StatusData.SetBit(1, value);
        }
        
        public void SetDataBit2(bool value)
        {
            _link.StatusData.SetBit(2, value);
        }
        
        public void SetDataBit3(bool value)
        {
            _link.StatusData.SetBit(3, value);
        }
        
        public void SetDataBit4(bool value)
        {
            _link.StatusData.SetBit(4, value);
        }
        
        public void SetDataBit5(bool value)
        {
            _link.StatusData.SetBit(5, value);
        }
        
        public void SetDataBit6(bool value)
        {
            _link.StatusData.SetBit(6, value);
        }
        
        public void SetDataBit7(bool value)
        {
            _link.StatusData.SetBit(7, value);
        }
    }
}
