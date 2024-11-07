using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Interactions.UIElements
{
    public class SwitchRotary : VisualElement
    {
        public int Index
        {
            get => _actualIndex;
            set
            {
                if (_actualIndex == value) return;
                _actualIndex = value;
                _index.text = _actualIndex.ToString();
            }
        }

        public float Angle
        {
            get => _angle;
            set
            {
                if (System.Math.Abs(_angle - value) < 1e-3) return;
                _angle = value;
                _knob.style.rotate = new Rotate(UnityEngine.UIElements.Angle.Degrees(_angle));
            }
        }

        public event Action Clicked
        {
            add
            {
                if (_clickable == null)
                {
                    _clickable = new Clickable(value)
                    {
                        target = _container
                    };
                }
                else
                {
                    _clickable.clicked += value;
                }
            }
            remove
            {
                if (_clickable == null) return;
                _clickable.clicked -= value;
            }
        }
        
        private const string UXML = "UXML/panel-switch-rotary";
        
        private Clickable _clickable;
        private readonly VisualElement _container;
        private readonly VisualElement _knob;
        private readonly Label _label;
        private readonly Label _index;

        private int _actualIndex;
        private float _angle;

        public SwitchRotary(string name)
        {
            var container = new VisualElement();
            Resources.Load<VisualTreeAsset>(UXML).CloneTree(container);
            
            hierarchy.Add(container);
            
            _container = container.Q("container");
            _label = container.Q<Label>("label");
            _knob = container.Q("knob");
            _index = container.Q<Label>("index");
            
            _label.text = name.ToUpper();
        }

        public void Bind(Interactions.SwitchRotary switchRotary)
        {
            _label.text = switchRotary.name.ToUpper();
            Index = switchRotary.Index.Value;
            Angle = switchRotary.Angle.Value;

            Clicked += switchRotary.Click; 
            
            switchRotary.OnStateChanged += (index, angle) =>
            {
                Index = index;
                Angle = angle;
            }; 
        }
    }
}