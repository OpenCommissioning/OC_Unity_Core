using System;
using System.Collections.Generic;
using System.IO;
using OC.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class ProductTemplateEditor : EditorWindow
    {
        private const string UXML = "UXML/ProductData/ProductTemplateEditor";

        private readonly List<EntryData> _entryData = new ();

        private VisualElement _file;
        private VisualElement _empty;
        private Button _buttonSave;
        private Button _buttonCreate;
        private Label _textName;
        private ListView _listView;
        private string _fileName;

        private bool _ignoreDirtyFlag;

        private SerializedObject _serializedObject;

        [MenuItem("Open Commissioning/Product Data/Template Editor", priority = 100)]
        private static void Init()
        {
            var window = GetWindow<ProductTemplateEditor>();
            window.titleContent = new GUIContent("Product Template Editor");
        }

        private void CreateGUI()
        {
            Resources.Load<VisualTreeAsset>(UXML).CloneTree(rootVisualElement);
            _serializedObject = new SerializedObject(this);
            _serializedObject.ApplyModifiedProperties();
            
            _empty = rootVisualElement.Q("empty");
            _file = rootVisualElement.Q("file");

            var toolbar = rootVisualElement.Q<ToolbarMenu>();
            toolbar.menu.AppendAction("Create",_ => Create(), _ => DropdownMenuAction.Status.Normal);
            toolbar.menu.AppendAction("Open",_ => Open(), _ => DropdownMenuAction.Status.Normal);
            toolbar.menu.AppendAction("Save",_ => Save(), _ => DropdownMenuAction.Status.Normal);
            toolbar.menu.AppendAction("Clear",_ => Clear(), _ => DropdownMenuAction.Status.Normal);

            _textName = rootVisualElement.Q<Label>("name");
            _buttonSave = rootVisualElement.Q<Button>("save");
            _buttonCreate = rootVisualElement.Q<Button>("create");

            VisualElement MakeItem() => new EntryDataElement();
            void BindItem(VisualElement e, int i) => (e as EntryDataElement)?.Bind(_entryData[i]);
            
            _listView = rootVisualElement.Q<ListView>("list");
            _listView.showFoldoutHeader = true;
            _listView.itemsSource = _entryData;
            _listView.makeItem = MakeItem;
            _listView.bindItem = BindItem;
            _listView.itemsAdded += ListViewOnItemsAdded;

            _buttonSave.clicked += Save;
            _buttonCreate.clicked += Create;

            EnableEmptyState(true);
            SetDirty(false);
            
            rootVisualElement.TrackSerializedObjectValue(_serializedObject, CheckDirtyState);
        }

        private void ListViewOnItemsAdded(IEnumerable<int> obj)
        {
            foreach (var index in obj)
            {
                _entryData[index] = new EntryData
                {
                    Key = $"New key {index}"
                };
            }
        }

        private void Create()
        {
            Clear();
            
            _fileName = "New Template";
            _ignoreDirtyFlag = true;

            EnableEmptyState(false);
            SetDirty(false);
        }
        
        private void Open()
        {
            var path = UnityEditor.EditorUtility.OpenFilePanel("Open Product Template", "", "xml");
            if (string.IsNullOrEmpty(path)) return;

            try
            {
                Clear();
                EnableEmptyState(false);
                SetDirty(false);
                _ignoreDirtyFlag = true;

                _fileName = Path.GetFileName(path);
                var entryData = ProductDataFactory.GetTemplateContent(path);
                foreach (var data in entryData)
                {
                    _entryData.Add(data);
                }
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, exception);
            }
        }

        private void Save()
        {
            var path = UnityEditor.EditorUtility.SaveFilePanel("Save Product Template", "", "Typ.xml", "xml");
            if (string.IsNullOrEmpty(path)) return;
            ProductDataFactory.CreateTemplate(path, _entryData);
            
            _fileName = Path.GetFileName(path);
            EnableEmptyState(false);
            SetDirty(false);
        }

        private void Clear()
        {
            _entryData.Clear();
            _listView.Rebuild();
            EnableEmptyState(true);
            SetDirty(false);
        }
        
        private void CheckDirtyState(SerializedObject serializedObject)
        {
            if (_entryData is null || _entryData.Count == 0) return;
            if (_ignoreDirtyFlag)
            {
                _ignoreDirtyFlag = false;
                return;
            }
            
            SetDirty(true);
        }

        private void EnableEmptyState(bool enable)
        {
            _empty.SetEnabled(enable);
            _empty.style.display = enable ? DisplayStyle.Flex : DisplayStyle.None;
            
            _file.SetEnabled(!enable);
            _file.style.display = enable ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void SetDirty(bool dirty)
        {
            if (dirty)
            {
                _textName.text = $"{_fileName}*";
                _buttonSave.SetEnabled(true);
            }
            else
            {
                _textName.text = _fileName;
                _buttonSave.SetEnabled(false);
            }
        }
    }
}