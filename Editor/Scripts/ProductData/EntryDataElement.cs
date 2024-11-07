using System;
using OC.Data;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class EntryDataElement : VisualElement
    {
        private EntryData _entryData;
        private readonly Foldout _foldout;
        private readonly TextField _textFieldKey;
        private readonly EnumField _enumFieldType;
        private readonly TextField _textFieldValue;
        private readonly TextField _textFieldInfo;
        private bool _ignoreEnumCallback;

        public EntryDataElement()
        {
            var container = new VisualElement
            {
                style =
                {
                    paddingLeft = new StyleLength(18)
                }
            };

            _foldout = new Foldout();
            _textFieldKey = new TextField("Key");
            _enumFieldType = new EnumField("Type", EntryDataType.CHARS);
            
            _textFieldValue = new TextField("Value")
            {
                isDelayed = true
            };
            _textFieldInfo = new TextField("Info")
            {
                isReadOnly = true
            };
            
            _foldout.Add(_textFieldKey);
            _foldout.Add(_enumFieldType);
            _foldout.Add(_textFieldValue);
            _foldout.Add(_textFieldInfo);
            container.Add(_foldout);
            hierarchy.Add(container);
        }

        public void Bind(EntryData entryData)
        {
            _entryData = entryData;
            
            _enumFieldType.Init(entryData.Type);
            
            //BUG: bloody Unity can not handle ChangeEvent<Enum>
            //SetValueWithoutNotify for EnumField not working properly. ChangeEvent will be triggered
            //This bool flag "_ignoreEnumCallback" is workaround for ChangeEvent call
            //https://forum.unity.com/threads/registercallback-for-changeevent-dealing-with-enums.692455/#post-7758939
            _ignoreEnumCallback = true;
            _enumFieldType.UnregisterCallback<ChangeEvent<string>>(TypeCallback);

            _foldout.text = _entryData.Key;
            _textFieldKey.SetValueWithoutNotify(_entryData.Key);
            _textFieldValue.SetValueWithoutNotify(_entryData.Value);
            _textFieldInfo.SetValueWithoutNotify(GetTypeInfo(_entryData.Type, _entryData.Value));

            _enumFieldType.RegisterCallback<ChangeEvent<string>>(TypeCallback);
            
            _textFieldKey.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                _entryData.Key = evt.newValue;
            });

            _textFieldValue.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                var type = (EntryDataType)_enumFieldType.value;

                if (type == EntryDataType.CHARS)
                {
                    _textFieldInfo.value = GetTypeInfo(type, _textFieldValue.text);
                }

                if (entryData.Validate(_textFieldValue.value))
                {
                    _textFieldValue.SetValueWithoutNotify(evt.newValue);
                    _entryData.Value = evt.newValue;
                }
                else
                {
                    _textFieldValue.SetValueWithoutNotify(evt.previousValue);
                }
            });
            
        }

        private void TypeCallback(ChangeEvent<string> evt)
        {
            if (_ignoreEnumCallback)
            {
                _ignoreEnumCallback = false;
                return;
            }

            if (!Enum.TryParse(evt.newValue, out EntryDataType type)) return;
            
            _entryData.Type = type;
            _textFieldInfo.SetValueWithoutNotify(GetTypeInfo(_entryData.Type, _entryData.Value));

            if (!_entryData.Validate(_entryData.Value))
            {
                _entryData.Value = "";
                _textFieldValue.SetValueWithoutNotify(_entryData.Value);
            }
        }

        private static string GetTypeInfo(EntryDataType type, string value)
        {
            return type switch
            {
                EntryDataType.CHARS => $"CHARS array length: {value.Length}",
                EntryDataType.BYTES => "Hex value format 0xAA (1 byte), 0xAABB (2 bytes)",
                EntryDataType.INT16 => $"INT16 value format range: [{short.MinValue}, {short.MaxValue}]",
                EntryDataType.INT32 => $"INT32 value format range: [{int.MinValue}, {int.MaxValue}]",
                EntryDataType.UINT16 => $"UINT16 value format range: [{ushort.MinValue}, {ushort.MaxValue}]",
                EntryDataType.UINT32 => $"UINT32 value format range: [{uint.MinValue}, {uint.MaxValue}]",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}