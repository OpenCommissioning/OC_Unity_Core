using System;
using OC.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class EntryDataElementReadonly : VisualElement
    {
        private EntryData _entryData;
        private readonly Foldout _foldout;
        private readonly TextField _textFieldKey;
        private readonly TextField _textFieldType;
        private readonly TextField _textFieldValue;
        private readonly TextField _textFieldInfo;

        public EntryDataElementReadonly()
        {
            var container = new VisualElement
            {
                style =
                {
                    paddingLeft = new StyleLength(18)
                }
            };

            _foldout = new Foldout();
            _textFieldKey = new TextField("Key")
            {
                isReadOnly = true
            };
            _textFieldType = new TextField("Type")
            {
                isReadOnly = true
            };
            _textFieldValue = new TextField("Value")
            {
                isDelayed = true
            };
            _textFieldInfo = new TextField("Info")
            {
                isReadOnly = true
            };
            
            _foldout.Add(_textFieldKey);
            _foldout.Add(_textFieldType);
            _foldout.Add(_textFieldValue);
            _foldout.Add(_textFieldInfo);
            container.Add(_foldout);
            hierarchy.Add(container);
        }

        public void Bind(EntryData entryData, ProductDataViewer productDataViewer)
        {
            _entryData = entryData;
            _foldout.text = entryData.Key;
            _textFieldKey.SetValueWithoutNotify(_entryData.Key);
            _textFieldValue.SetValueWithoutNotify(_entryData.Value);
            _textFieldType.SetValueWithoutNotify(_entryData.Type.ToString());
            _textFieldInfo.SetValueWithoutNotify(GetTypeInfo(_entryData.Type, _entryData.Value));

            _textFieldValue.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                if (_entryData.Type is EntryDataType.CHARS or EntryDataType.BYTES)
                {
                    if (evt.previousValue.Length != evt.newValue.Length)
                    {
                        _textFieldValue.SetValueWithoutNotify(evt.previousValue);
                        Logging.Logger.Log(LogType.Error, $"{_entryData.Key}: Length of data array can't be changed!");
                        return;
                    }
                }
                
                if (_entryData.Type == EntryDataType.CHARS)
                {
                    _textFieldInfo.value = GetTypeInfo(_entryData.Type, _textFieldValue.text);
                }

                if (_entryData.Validate(_textFieldValue.value))
                {
                    _textFieldValue.SetValueWithoutNotify(evt.newValue);
                    _entryData.Value = evt.newValue;
                    productDataViewer.SetDirty(true);
                }
                else
                {
                    _textFieldValue.SetValueWithoutNotify(evt.previousValue);
                }
            });
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