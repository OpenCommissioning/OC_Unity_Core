using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OC.Communication;
using OC.Components;

namespace OC.Interactions
{
    [AddComponentMenu("Open Commissioning/Interactions/Lamp")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class Lamp : Device, ICustomInspector, IControlOverridable
    {
        public bool Signal
        {
            get => _value.Value;
            set => _value.Value = value;
        }
        
        public override int AllocatedBitLength => 1;
        public IProperty<bool> Override => _override;
        public IProperty<bool> Value => _value;
        public IPropertyReadOnly<Color> Color => _color;

        [SerializeField] 
        protected Property<bool> _override = new(false);
        [SerializeField] 
        private Property<bool> _value = new(false);
        [SerializeField] 
        private Property<Color> _color = new(UnityEngine.Color.cyan);
        [SerializeField] 
        protected List<ColorChanger> _colorChangers = new();

        public UnityEvent<bool> OnValueChanged;

        private new void Start()
        {
            base.Start();
            _value.ValueChanged += OnValueChangedAction;
        }

        private void OnDestroy()
        {
            _value.ValueChanged -= OnValueChangedAction;
        }
        
        protected override void Reset()
        {
            _link = new Link(this, "FB_Lamp");
        }

        private void LateUpdate()
        {
            if (!_override) _value.Value = Connector.Control.GetBit(0);
        }

        private void OnValidate()
        {
            _value.OnValidate();
            _color.OnValidate();

            foreach (var colorChanger in _colorChangers)
            {
                if (colorChanger == null) continue;
                colorChanger.Color = _color.Value.ScaleRGB(0.5f);
            }
        }

        private void OnValueChangedAction(bool value)
        {
            OnValueChanged?.Invoke(value);

            foreach (var colorChanger in _colorChangers)
            {
                colorChanger.Enable = value;
            }
        }
    }
}
