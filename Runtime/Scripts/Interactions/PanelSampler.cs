using System.Collections.Generic;
using System.Linq;
using OC.Communication;
using OC.Components;
using OC.Interactions.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Interactions
{
    [AddComponentMenu("Open Commissioning/Interactions/Panel Sampler")]
    [DefaultExecutionOrder(10)]
    public class PanelSampler : MonoComponent, IIndustrialPanel, ICustomInspector
    {
        public Link Link => _link;
        public IProperty<bool> Override => _override;
        public List<SampleDevice> Components => _components;

        [SerializeField]
        private string _name;
        [SerializeField]
        private List<SampleDevice> _components = new ();
        [SerializeField]
        protected Property<bool> _override = new (false);
        [SerializeField]
        private LinkDataDWord _link = new("FB_Panel");
        
        private int _bitLength;
        private bool _isValid;
        
        private const int BIT_LENGTH_MAX = 32;

        private void Start()
        {
            _link.Initialize(this);
            CheckSlots();
        }

        private void LateUpdate()
        {
            if (!_isValid) return;
            if (!_link.Enable) return;
            
            var index = 0;
            foreach (var item in _components)
            {
                for (var j = 0; j < item.AllocatedBitLength; j++)
                {
                    _link.StatusData.SetBit(index, item.Link.Status.GetBit(j));
                    item.Link.Control.SetBit(j, _link.ControlData.GetBit(index));
                    index++;
                }
            }
        }
        
        public VisualElement Create()
        {
            var groupName = string.IsNullOrEmpty(_name) ? gameObject.name : _name;
            var group = new IndustrialPanel(groupName);

            foreach (var visualElement in _components.Select(Factory.Create))
            {
                group.Add(visualElement);
            }

            return group;
        }

        private void CheckSlots()
        {
            if (_components.Count <= 0) return;
            
            _bitLength = _components.Sum(item => item.AllocatedBitLength);
            if (_bitLength > BIT_LENGTH_MAX)
            {
                Logging.Logger.Log(LogType.Error, $"Interface length {_bitLength} [bit] is out of the range [0..{BIT_LENGTH_MAX}]!", this);
                return;
            }

            _isValid = true;

            if (!_link.Enable) return;
            foreach (var device in _components)
            {
                if (!device.Link.Enable) continue;
                device.Link.Enable = false;
                Logging.Logger.Log(LogType.Warning, $"Panel {name} override {device.name} link state to disable", device);
            }
        }
    }
}
