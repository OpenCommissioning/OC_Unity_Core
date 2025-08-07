using System.Collections.Generic;
using System.IO;
using System.Linq;
using OC.Data;
using OC.MaterialFlow;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public class ProductDataViewer : EditorWindow
    {
        private const string UXML = "UXML/ProductData/ProductDataViewer";

        private readonly List<EntryData> _entryData = new ();
        
        private VisualElement _file;
        private VisualElement _empty;
        private DropdownField _dropdownFieldDirectory;
        private Button _buttonOverwrite;
        private Label _textName;
        private ListView _listView;
        private string _fileName;
        
        private SerializedObject _serializedObject;

        
        private List<ProductDataDirectory> _directories;
        private PayloadTag _payloadTag;
        private int _selectedDirectoryIndex;


        [MenuItem("Open Commissioning/Product Data/Data Viewer", priority = 100)]
        private static void Init()
        {
            var window = GetWindow<ProductDataViewer>();
            window.titleContent = new GUIContent("Product Data Viewer");
        }
        
        private void CreateGUI()
        {
            Resources.Load<VisualTreeAsset>(UXML).CloneTree(rootVisualElement);
            _serializedObject = new SerializedObject(this);
            _serializedObject.ApplyModifiedProperties();
            
            _empty = rootVisualElement.Q("empty");
            _file = rootVisualElement.Q("file");
            _dropdownFieldDirectory = rootVisualElement.Q<DropdownField>("directory");

            var toolbar = rootVisualElement.Q<ToolbarMenu>();
            toolbar.menu.AppendAction("Overwrite",_ => Overwrite(), _ => DropdownMenuAction.Status.Normal);
            toolbar.menu.AppendAction("Clear",_ => Clear(), _ => DropdownMenuAction.Status.Normal);

            _textName = rootVisualElement.Q<Label>("name");
            _buttonOverwrite = rootVisualElement.Q<Button>("overwrite");

            VisualElement MakeItem() => new EntryDataElementReadonly();
            void BindItem(VisualElement e, int i) => (e as EntryDataElementReadonly)?.Bind(_entryData[i], this);

            _listView = rootVisualElement.Q<ListView>("list");
            _listView.itemsSource = _entryData;
            _listView.makeItem = MakeItem;
            _listView.bindItem = BindItem;

            _buttonOverwrite.clicked += Overwrite;

            _dropdownFieldDirectory.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                var directory = _directories.FirstOrDefault(directory => directory.Name == evt.newValue);
                LoadEntryData(_payloadTag, directory);
            });

            EnableEmptyState(true);
            SetDirty(false);
        }

        private void OnSelectionChange()
        {
            if (!Application.isPlaying) return;
            if (Selection.activeGameObject == null) return;
            if (!Selection.activeGameObject.activeInHierarchy) return;
            if (!Selection.activeGameObject.TryGetComponent(out PayloadTag payloadTag)) return;
            if (_payloadTag == payloadTag) return;
            GetPayloadTagData(payloadTag);
            EnableEmptyState(false);
            SetDirty(false);
        }

        private void GetPayloadTagData(PayloadTag payloadTag)
        {
            _payloadTag = payloadTag;
            _directories = ProductDataDirectoryManager.Instance.GetValidDataDirectories(payloadTag.DirectoryId);
            if (_directories.Count == 0) return;
            _dropdownFieldDirectory.choices = _directories.Select(directory => directory.Name).ToList();
            _dropdownFieldDirectory.value = _directories[0].Name;
            LoadEntryData(payloadTag, _directories[0]);
        }

        private void LoadEntryData(PayloadTag payloadTag, ProductDataDirectory productDataDirectory)
        {
            if (_payloadTag is null) return;

            _selectedDirectoryIndex = _directories.IndexOf(productDataDirectory);
            
            _entryData.Clear();

            var entryData = payloadTag.GetProductDataContent(_selectedDirectoryIndex);
            foreach (var data in entryData)
            {
                _entryData.Add(data);
            }
            _listView.Rebuild();

            if (Path.IsPathRooted(productDataDirectory.GetValidPath()))
            {
                _fileName = $@"{productDataDirectory.GetValidPath()}\ID{payloadTag.Payload.UniqueId}.{ProductDataFactory.DATA_EXTENSION}";
            }
            else
            {
                _fileName = $@"{Application.dataPath}\{productDataDirectory.GetValidPath()}\ID{payloadTag.Payload.UniqueId}.{ProductDataFactory.DATA_EXTENSION}";  
            }
        }

        private void Overwrite()
        {
            _payloadTag.OverwriteProductData(_selectedDirectoryIndex, _entryData);
            SetDirty(false);
        }
        
        private void Clear()
        {
            _entryData.Clear();
            _listView.Rebuild();
            EnableEmptyState(true);
            SetDirty(false);
        }
        
        private void EnableEmptyState(bool enable)
        {
            _empty.SetEnabled(enable);
            _empty.style.display = enable ? DisplayStyle.Flex : DisplayStyle.None;
            
            _file.SetEnabled(!enable);
            _file.style.display = enable ? DisplayStyle.None : DisplayStyle.Flex;
        }

        public void SetDirty(bool dirty)
        {
            if (dirty)
            {
                _textName.text = $"{_fileName}*";
                _buttonOverwrite.SetEnabled(true);
            }
            else
            {
                _textName.text = _fileName;
                _buttonOverwrite.SetEnabled(false);
            }
        }
    }
}