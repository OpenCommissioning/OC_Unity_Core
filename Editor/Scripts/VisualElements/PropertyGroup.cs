using System.Collections.Generic;
using OC.Communication;
using OC.VisualElements;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class PropertyGroup : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<PropertyGroup, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _label = new() { name = "label", defaultValue = "Label" };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is not PropertyGroup group) return;
                group.Label = _label.GetValueFromBag(bag, cc); 
            }
        }

        public string Label
        {
            get => _labelElement.text;
            set => _labelElement.text = value;
        }

        private readonly Label _labelElement;
        private readonly VisualElement _options;
        private readonly VisualElement _content;
        private OverrideController _overrideController; 
        
        private const string USS = "StyleSheet/oc-inspector";
        private const string USS_CLASS_NAME = "property-group";
        private const string HEADER_USS_CLASS_NAME = USS_CLASS_NAME + "__header";
        private const string LINE_USS_CLASS_NAME = USS_CLASS_NAME + "__line";
        private const string OPTIONS_USS_CLASS_NAME = USS_CLASS_NAME + "__options";
        private const string LABEL_USS_CLASS_NAME = USS_CLASS_NAME + "__label";
        private const string CONTENT_USS_CLASS_NAME = USS_CLASS_NAME + "__content";

        public PropertyGroup() : this ("Label"){}

        public PropertyGroup(string text)
        {
            styleSheets.Add(Resources.Load<StyleSheet>(USS));
            
            AddToClassList(USS_CLASS_NAME);

            var header = new VisualElement();
            header.AddToClassList(HEADER_USS_CLASS_NAME);
            _content = new VisualElement();
            _content.AddToClassList(CONTENT_USS_CLASS_NAME);

            _labelElement = new Label(text);
            _labelElement.AddToClassList(LABEL_USS_CLASS_NAME);

            var line = new VisualElement();
            line.AddToClassList(LINE_USS_CLASS_NAME);
            
            _options = new VisualElement();
            _options.AddToClassList(OPTIONS_USS_CLASS_NAME);
            
            header.Add(_labelElement);
            header.Add(line);
            header.Add(_options);
            
            hierarchy.Add(header);
            hierarchy.Add(_content);
        }

        public new void Add(VisualElement visualElement)
        {
            _content.Add(visualElement);
        }
        
        public void AddHeaderElement(VisualElement visualElement)
        {
            _options.Add(visualElement);
        } 

        public void AddLinkOverride(IDevice link)
        {
            _overrideController = new OverrideController(link, _content);
            _options.Add(_overrideController);
        }
    }
}
