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
    public class PanelSampler : MonoComponent, IDevice, ICustomInspector, IIndustrialPanel
    {
        public Link Link => _link;
        public List<Device> Components => _components;

        [SerializeField]
        private string _name;
        [SerializeField]
        private List<Device> _components = new ();
        [SerializeField]
        private Link _link;

        private ConnectorDataDWord _connectorDataDWord;
        private int _bitLength;
        private int _bitLengthMax;
        private bool _isValid;

        private void Start()
        {
            InitializeLink();
            CheckSlots();
        }
        
        private void Reset()
        {
            _link = new Link(this, "FB_Panel");
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
                    _connectorDataDWord.StatusData.SetBit(index,item.Connector.Status.GetBit(j));
                    item.Connector.Control.SetBit(j,_connectorDataDWord.ControlData.GetBit(index));
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
            if (_bitLength > _bitLengthMax)
            {
                Logging.Logger.Log(LogType.Error, $"Interface length {_bitLength} [bit] is out of the range [0..{_bitLengthMax}]!", this);
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

        private void InitializeLink()
        {
            _link.Initialize(this);
            _connectorDataDWord = new ConnectorDataDWord(_link);
            _bitLengthMax = sizeof(uint) * 8;
        }
    }
}
