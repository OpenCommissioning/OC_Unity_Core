using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Interactions.UIElements
{
    public class Lamp : VisualElement
    {
        public bool Value
        {
            get => _value;
            set
            {
                if (_value == value) return;
                _value = value;
                SetLampStyle(_value);
            }

        }
        
        public Color Color
        {
            get => _color;
            set
            {
                if (_color == value) return;
                SetColor(value);
            }
        }

        private bool _value;
        private Color _color;

        private const string UXML = "UXML/panel-lamp";

        private readonly VisualElement _lamp;
        private StyleColor _initStyleColor;
        private StyleColor _enableStyleColor;

        public Lamp(string name)
        {
            var container = new VisualElement();
            Resources.Load<VisualTreeAsset>(UXML).CloneTree(container);
            hierarchy.Add(container);
            container.Q<Label>("label").text = name.ToUpper();
            _lamp = container.Q("lamp");
            _initStyleColor = _enableStyleColor = _lamp.style.backgroundColor;
        }

        public void Bind(Interactions.Lamp lamp)
        {
            Color = lamp.Color.Value;
            Value = lamp.Value.Value;

            lamp.Value.ValueChanged += value => Value = value;
            lamp.Color.ValueChanged += value => Color = value;
        }
        
        private void SetLampStyle(bool active)
        {
            _lamp.style.backgroundColor = active ? _enableStyleColor : _initStyleColor;
        }

        private void SetColor(Color color)
        {
            _color = color;
            _initStyleColor.value = _color.ScaleRGB(0.4f);
            _enableStyleColor.value = _color;
            SetLampStyle(_value);
        }
    } 
}



