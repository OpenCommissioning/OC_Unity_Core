using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class ProgressBar : UnityEngine.UIElements.ProgressBar
    {
        public new class UxmlFactory : UxmlFactory<ProgressBar, UxmlTraits> { }

        public new class UxmlTraits : UnityEngine.UIElements.ProgressBar.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription _showLimits = new() { name = "ShowLimits", defaultValue = false };
            private readonly UxmlColorAttributeDescription _colorProgressBar = new() { name = "Color-Bar", defaultValue = Color.white };
            private readonly UxmlColorAttributeDescription _colorBackground = new() { name = "Color-Background", defaultValue = new Color(0.5f,0.5f,0.5f, 0.5f) };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not ProgressBar progressBar) return;
                progressBar.ShowLimits = _showLimits.GetValueFromBag(bag, cc);
                progressBar.ColorBar = _colorProgressBar.GetValueFromBag(bag, cc);
                progressBar.ColorBackground = _colorBackground.GetValueFromBag(bag, cc);
            }
        }

        public Color ColorBar
        {
            get => _colorBar;
            set
            {
                _progressBar.style.backgroundColor = new StyleColor(value);
                _colorBar = value;
            }
        }

        public Color ColorBackground
        {
            get => _colorBackground;
            set
            {
                _background.style.backgroundColor = new StyleColor(value);
                _colorBackground = value;
            }
        }

        public bool ShowLimits
        {
            get => _showLimits;
            set
            {
                if (_showLimits == value) return;
                _showLimits = value;
                EnableLimits(value);
            }
        }

        public bool Min
        {
            get => _min;
            set
            {
                if (_min == value) return;
                _min = value;
                _indicatorMin.EnableInClassList(LIMIT_ACTIVE_USS_CLASS_NAME, value);
            }
        }
        
        public bool Max
        {
            get => _max;
            set
            {
                if (_max == value) return;
                _max = value;
                _indicatorMax.EnableInClassList(LIMIT_ACTIVE_USS_CLASS_NAME, value);
            }
        }

        private Color _colorBar;
        private Color _colorBackground;
        
        private readonly VisualElement _progressBar;
        private readonly VisualElement _background;
        private readonly VisualElement _container;
        private VisualElement _indicatorMin;
        private VisualElement _indicatorMax;

        private bool _showLimits;
        private bool _min;
        private bool _max;

        private const string USS = "StyleSheet/oc-inspector";
        private const string USS_CLASS_NAME = "progress-bar";
        private const string CONTAINER_USS_CLASS_NAME = "progress-bar__container";
        private const string LIMIT_USS_CLASS_NAME = "progress-bar__limit";
        private const string LIMIT_ACTIVE_USS_CLASS_NAME = "progress-bar__limit-active";

        public ProgressBar() : this(""){}
        
        public ProgressBar(string title)
        {
            this.title = title;
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            AddToClassList(USS_CLASS_NAME);
            AddToClassList("unity-base-field__inspector_field");

            lowValue = 0;
            highValue = 1;

            _progressBar = this.Q<VisualElement>(className: progressUssClassName);
            _background = this.Q<VisualElement>(className: backgroundUssClassName);
            _background.style.flexGrow = 1;

            _indicatorMin = new VisualElement{name = "indicator_min"};
            _indicatorMin.AddToClassList(LIMIT_USS_CLASS_NAME);
            _indicatorMax = new VisualElement{name = "indicator_max"};
            _indicatorMax.AddToClassList(LIMIT_USS_CLASS_NAME);
            
            _container = this.Q<VisualElement>(className: containerUssClassName);
            _container.AddToClassList(CONTAINER_USS_CLASS_NAME);
        }

        private void Refresh(ChangeEvent<float> evt)
        {
            Min = evt.newValue <= lowValue;
            Max = evt.newValue >= highValue;
        }

        private void EnableLimits(bool enable)
        {
            if (enable)
            {
                if (_indicatorMin == null)
                {
                    _indicatorMin = new VisualElement{name = "indicator_min"};
                    _indicatorMin.AddToClassList(LIMIT_USS_CLASS_NAME);
                }
                
                if (_indicatorMax == null)
                {
                    _indicatorMax = new VisualElement{name = "indicator_max"};
                    _indicatorMax.AddToClassList(LIMIT_USS_CLASS_NAME);
                }
                
                if (!_container.Contains(_indicatorMin)) _container.Add(_indicatorMin);
                if (!_container.Contains(_indicatorMax)) _container.Add(_indicatorMax);
                
                _indicatorMin.SendToBack();
                _indicatorMax.BringToFront();
                
                this.RegisterValueChangedCallback(Refresh);
            }
            else
            {
                this.UnregisterValueChangedCallback(Refresh);

                if (_indicatorMin != null && _container.Contains(_indicatorMin)) _indicatorMin.RemoveFromHierarchy();
                if (_indicatorMax != null && _container.Contains(_indicatorMax)) _indicatorMax.RemoveFromHierarchy();
            }
        }
    }
}