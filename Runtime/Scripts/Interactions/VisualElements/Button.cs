using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Interactions.UIElements
{
    public class Button : VisualElement
    {
        private event Action Down;
        private event Action Up;

        public bool Value
        {
            get => _value;
            set
            {
                if (_value == value) return;
                SetValueWithoutNotify(value);
            }
        }

        public bool Feedback
        {
            get => _feedback;
            set
            {
                if (_feedback == value) return;
                _feedback = value;
                SetLampActiveState(value);
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

        public Interactions.Button.UIStyle VisualStyle
        {
            get => _visualStyle;
            set
            {
                if (_visualStyle == value) return;
                SetVisualStyle(value);
            }
        }

        private bool _value;
        private bool _feedback;
        private Color _color;
        private Interactions.Button.UIStyle _visualStyle;

        private readonly MouseEvents _mouseEvents;

        private const string UXML = "UXML/panel-button";
        private const string USS_BUTTON_ACTIVE = "button-active";
        private const string CSS_SAFETY = "StyleSheet/industrial-button-estop";

        private readonly Label _label;
        private readonly VisualElement _lamp;
        private readonly VisualElement _button;
        private readonly StyleColor _initLampStyleColor;
        private readonly StyleSheet _styleSheetSafety;
        
        private StyleColor _enableLampStyleColor;
        private StyleColor _initButtonStyleColor;
        private StyleColor _enableButtonStyleColor;

        public Button() : this("Button"){}
        
        public Button(string name)
        {
            var container = new VisualElement();
            Resources.Load<VisualTreeAsset>(UXML).CloneTree(container);
            _styleSheetSafety = Resources.Load<StyleSheet>(CSS_SAFETY);

            hierarchy.Add(container);

            _label = container.Q<Label>("label");
            _lamp = container.Q("lamp");
            _button = container.Q("button");

            _label.text = name.ToUpper();
            _initLampStyleColor = _enableLampStyleColor = _lamp.style.backgroundColor;
            _initButtonStyleColor = _enableButtonStyleColor = _button.style.backgroundColor;

            _mouseEvents = new MouseEvents()
            {
                target = _button
            };

            _mouseEvents.Up += () => Up?.Invoke();
            _mouseEvents.Down += () => Down?.Invoke();
        }

        public void Bind(Interactions.Button button)
        {
            _label.text = button.name.ToUpper();
            Color = button.Color.Value;
            VisualStyle = button.VisualStyle.Value;
            Value = button.Pressed.Value;
            Feedback = button.Feedback.Value;

            Down += button.Press;
            Up += button.Release;

            button.Pressed.ValueChanged += value => Value = value;
            button.Feedback.ValueChanged += value => Feedback = value;
            button.Color.ValueChanged += value => Color = value;
            button.VisualStyle.ValueChanged += value => VisualStyle = value;
        }

        private void SetValueWithoutNotify(bool value)
        {
            _value = value;
            SetButtonActiveState(value);
        }

        private void SetButtonActiveState(bool active)
        {
            if (active)
            {
                _button.AddToClassList(USS_BUTTON_ACTIVE);
                _button.style.backgroundColor = _enableButtonStyleColor;
            }
            else
            {
                _button.RemoveFromClassList(USS_BUTTON_ACTIVE);
                _button.style.backgroundColor = _initButtonStyleColor;
            }
        }

        private void SetLampActiveState(bool active)
        {
            _lamp.style.backgroundColor = active ? _enableLampStyleColor : _initLampStyleColor;
        }

        private void SetColor(Color color)
        {
            _color = color;
            _enableLampStyleColor.value = _color;
            _initButtonStyleColor = _color.ScaleRGB(0.6f);
            _enableButtonStyleColor = _color.ScaleRGB(0.75f);
            SetButtonActiveState(_value);
        }

        private void SetVisualStyle(Interactions.Button.UIStyle visualStyle)
        {
            _visualStyle = visualStyle;
            styleSheets.Remove(_styleSheetSafety);
            
            switch (visualStyle)
            {
                case Interactions.Button.UIStyle.Default:
                    break;
                case Interactions.Button.UIStyle.Safety:
                    styleSheets.Add(_styleSheetSafety);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(visualStyle), visualStyle, null);
            }
        }
    }
}
