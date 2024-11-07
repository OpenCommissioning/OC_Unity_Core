using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using OC.Communication;
using OC.Components;

namespace OC.Interactions
{
    [AddComponentMenu("Open Commissioning/Actor/Button")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class Button : Device, ICustomInspector
    {
        public override int AllocatedBitLength => 1;
        public Property<bool> Pressed => _pressed;
        public Property<bool> Feedback => _feedback;
        public IPropertyReadOnly<bool> Value => _feedback;
        public IPropertyReadOnly<Color> Color => _color;
        public Property<UIStyle> VisualStyle => _visualStyle;

        [SerializeField] 
        protected Property<bool> _pressed = new (false);
        [SerializeField] 
        protected Property<bool> _feedback = new (false);

        [SerializeField] 
        protected bool _localFeedback;
        [SerializeField] 
        private ButtonType _type = ButtonType.Click;
        [SerializeField] 
        private Property<UIStyle> _visualStyle = UIStyle.Default;
        [SerializeField]
        protected Property<Color> _color = new (UnityEngine.Color.cyan);
        [SerializeField] 
        protected List<ColorChanger> _colorChangers = new ();

        public UnityEvent OnClickEvent;
        public UnityEvent<bool> OnPressedChanged;
        public UnityEvent<bool> OnFeedbackChanged;

        private const float CLICK_DURATION = 0.1f;

        private new void Start()
        {
            base.Start();
            _pressed.ValueChanged += PressedOnValueChanged;
            _feedback.ValueChanged += FeedbackOnValueChanged;
        }

        protected void OnDestroy()
        {
            _pressed.ValueChanged -= PressedOnValueChanged;
            _feedback.ValueChanged -= FeedbackOnValueChanged;
        }

        private void OnValidate()
        {
            _pressed.OnValidate();
            _feedback.OnValidate();
            _color.OnValidate();
            _visualStyle.OnValidate();
            
            foreach (var colorChanger in _colorChangers)
            {
                if (colorChanger == null) continue;
                colorChanger.Color = _color.Value.ScaleRGB(0.5f);
            }
        }

        protected override void Reset()
        {
            _link = new Link(this, "FB_Button");
        }

        public void Click()
        {
            if (!Application.isPlaying) return;
            switch (_type)
            {
                case ButtonType.Click:
                    StopAllCoroutines();
                    StartCoroutine(ClickCoroutine());
                    break;
                case ButtonType.Toggle:
                    _pressed.Value = !_pressed.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Press()
        {
            if (!Application.isPlaying) return;
            _pressed.Value = _type switch
            {
                ButtonType.Click => true,
                ButtonType.Toggle => !_pressed.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Release()
        {
            if (!Application.isPlaying) return;
            if (_type == ButtonType.Click) _pressed.Value = false;
        }

        private void LateUpdate()
        {
            if (!_localFeedback) _feedback.Value = Connector.Control.GetBit(0);
        }

        private void PressedOnValueChanged(bool value)
        {
            Connector.Status.SetBit(0, value);
            OnPressedChanged?.Invoke(value);
            if (value) OnClickEvent?.Invoke();
            if (_localFeedback) _feedback.Value = value;
        }
        
        private void FeedbackOnValueChanged(bool value)
        {
            OnFeedbackChanged?.Invoke(value);
            
            foreach (var colorChanger in _colorChangers)
            {
                colorChanger.Enable = value;
            }
        }
        
        private IEnumerator ClickCoroutine()
        {
            _pressed.Value = true;
            yield return new WaitForSeconds(CLICK_DURATION);
            _pressed.Value = false;
        }

        private enum ButtonType
        {
            Click,
            Toggle
        }

        public enum UIStyle
        {
            Default,
            Safety
        }
    }
}