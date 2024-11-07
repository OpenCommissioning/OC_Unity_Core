using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class LampField : BaseField<bool>
    {
        public new class UxmlFactory : UxmlFactory<LampField, UxmlTraits> { }

        public new class UxmlTraits : BaseFieldTraits<bool, UxmlBoolAttributeDescription>
        {
            private readonly UxmlEnumAttributeDescription<InspectorLampShape> _lampShape = new() { name = "lamp-shape", defaultValue = InspectorLampShape.Round};
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not LampField baseFieldLamp) return;
                baseFieldLamp.LampShape = _lampShape.GetValueFromBag(bag, cc); 
            }
        }
        
        public override bool value
        {
            get => base.value;
            set 
            {
                base.value = value;
                _visualInput.style.backgroundColor = value ? _activeColor : _defaultColor;
            }
        }

        public InspectorLampShape LampShape
        {
            get => _lampShape;
            set => SetLampShape(value);
        }

        private const string USS = "StyleSheet/oc-inspector";
        private const string USS_CLASS_NAME = "lamp";
        private const string LAMP_SHAPE_USS_CLASS_NAME = USS_CLASS_NAME + "__shape";
        private const string LAMP_SHAPE_ROUND_USS_CLASS_NAME = LAMP_SHAPE_USS_CLASS_NAME + "-round";
        private const string LAMP_SHAPE_SQUARE_USS_CLASS_NAME = LAMP_SHAPE_USS_CLASS_NAME + "-square";
        private const string LAMP_SHAPE_RECTANGLE_USS_CLASS_NAME = LAMP_SHAPE_USS_CLASS_NAME + "-rectangle";

        private readonly VisualElement _visualInput;
        private readonly StyleColor _defaultColor;
        private readonly StyleColor _activeColor;
        private InspectorLampShape _lampShape = InspectorLampShape.Round;
        
        public LampField() : this("", Color.green){}

        public LampField(string label, Color color) : base(label, null)
        {
            focusable = false;
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            AddToClassList(USS_CLASS_NAME);

            _visualInput = this.Q(className: inputUssClassName);
            _visualInput.AddToClassList(LAMP_SHAPE_USS_CLASS_NAME);
            
            _defaultColor = _visualInput.style.backgroundColor;
            _activeColor = new StyleColor(color);
            
            SetLampShape(_lampShape);
        }
        
        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);
            _visualInput.style.backgroundColor = newValue ? _activeColor : _defaultColor;
        }

        private void SetLampShape(InspectorLampShape shape)
        {
            _lampShape = shape;
            
            _visualInput.RemoveFromClassList(LAMP_SHAPE_ROUND_USS_CLASS_NAME);
            _visualInput.RemoveFromClassList(LAMP_SHAPE_SQUARE_USS_CLASS_NAME);
            _visualInput.RemoveFromClassList(LAMP_SHAPE_RECTANGLE_USS_CLASS_NAME);

            switch (shape)
            {
                case InspectorLampShape.Round:
                    _visualInput.AddToClassList(LAMP_SHAPE_ROUND_USS_CLASS_NAME);
                    break;
                case InspectorLampShape.Square:
                    _visualInput.AddToClassList(LAMP_SHAPE_SQUARE_USS_CLASS_NAME);
                    break;
                case InspectorLampShape.Rectangle:
                    _visualInput.AddToClassList(LAMP_SHAPE_RECTANGLE_USS_CLASS_NAME);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
            }
        }

        public enum InspectorLampShape
        {
            Round, 
            Square,
            Rectangle
        }
    }
}