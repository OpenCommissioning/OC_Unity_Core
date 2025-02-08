using System;
using OC.Components;
using UnityEngine;
using UnityEngine.Events;

namespace OC.Interactions
{
    [AddComponentMenu("Open Commissioning/Interactions/Switch Rotary")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class SwitchRotary : Switch, ICustomInspector
    {
        public IPropertyReadOnly<float> Angle => _angle;

        [Header("Rotary Settings")] 
        [SerializeField] 
        private Property<float> _angle = new ();
        [SerializeField] 
        private Vector2 _range = new (-45,45);
        [SerializeField] 
        private float _offset;

        private float _segmentStep;

        public event Action<int, float> OnStateChanged;
        public UnityEvent<float> OnRotationChanged;

        protected new void OnEnable()
        {
            base.OnEnable();
            _index.OnValueChanged += OnIndexChangedAction;
            OnValidate();
            OnIndexChangedAction(_index.Value);
        }

        protected new void OnDisable()
        {
            base.OnDisable();
            _index.OnValueChanged -= OnIndexChangedAction;
        }

        private new void OnValidate()
        {
            base.OnValidate();
            _range = new Vector2(Mathf.Min(_range.x, _range.y), Mathf.Max(_range.x, _range.y));
            _segmentStep = (_range.y - _range.x) / (_stateCount - 1);
            _angle.Value = _index.Value * _segmentStep + _range.x + _offset;
        }

        private void OnIndexChangedAction(int index)
        {
            _angle.Value = index * _segmentStep + _range.x + _offset;
            OnStateChanged?.Invoke(index, _angle);
            OnRotationChanged?.Invoke(_angle);
        }
    }
}
