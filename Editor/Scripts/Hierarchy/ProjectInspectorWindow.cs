using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;
using OC.Communication;
using OC.VisualElements;
using UnityEngine.SceneManagement;

namespace OC.Editor
{
    public class ProjectInspectorWindow : EditorWindow
    {
        private const string UXML = "UXML/ProjectInspector";
        
        private MultiColumnTreeView _multiColumnTreeView;
        private List<TreeViewItemData<HierarchyItem>> _treeViewData = new ();
        private ToolbarSearchField _toolbarSearchField;
        private ToolbarButton _toolbarButtonRefresh;
        private ToolbarButton _toolbarButtonReset;
        private string _searchQuery = string.Empty;
        
        [MenuItem("Open Commissioning/Project Inspector")]
        public static void ShowWindow()
        {
            var window = GetWindow<ProjectInspectorWindow>("Project Inspector");
            window.titleContent = new GUIContent("Project Inspector");
            window.Show();
        }
        
        private void CreateGUI()
        {
            Resources.Load<VisualTreeAsset>(UXML).CloneTree(rootVisualElement);
            
            _toolbarSearchField = rootVisualElement.Q<ToolbarSearchField>("toolbarSearchField");
            _toolbarButtonRefresh = rootVisualElement.Q<ToolbarButton>("toolbarButtonRefresh");
            _toolbarButtonReset = rootVisualElement.Q<ToolbarButton>("toolbarButtonReset");

            _toolbarSearchField.RegisterValueChangedCallback(OnSearchFilterChanged);
            _toolbarButtonRefresh.clicked += RefreshTreeViewDataSource;
            _toolbarButtonReset.clicked += ResetOverride;
            
            var content = rootVisualElement.Q("content");
            _multiColumnTreeView = CreateMultiColumnTreeView();
            content.Add(_multiColumnTreeView);

            RefreshTreeViewDataSource();
        }
        
        private void OnDisable()
        {
            _toolbarSearchField?.UnregisterValueChangedCallback(OnSearchFilterChanged);
            _toolbarButtonRefresh.clicked -= RefreshTreeViewDataSource;
            _toolbarButtonReset.clicked -= ResetOverride;
        }

        private MultiColumnTreeView CreateMultiColumnTreeView()
        {
            var multiColumnTreeView = new MultiColumnTreeView
            {
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
                autoExpand = true,
                reorderable = false,
                fixedItemHeight = 18f
            };

            var columnHierarchy = new Column()
            {
                title = "Hierarchy",
                stretchable = true,
                minWidth = 80f
            };
            columnHierarchy.bindCell += (element, i) =>
            {
                var item = multiColumnTreeView.GetItemDataForIndex<HierarchyItem>(i);
                if (element is Label label) label.text = item.Name;
            };
            
            var columnPath = new Column()
            {
                title = "Path",
                stretchable = true,
                minWidth = 80f
            };
            columnPath.bindCell += (element, i) =>
            {
                var item = multiColumnTreeView.GetItemDataForIndex<HierarchyItem>(i);
                if (element is Label label)
                {
                    if (item.Component == null)
                    {
                        label.style.display = DisplayStyle.None;
                    }
                    else
                    {
                        label.style.display = DisplayStyle.Flex;
                        if (item.Component is not IDevice device) return;
                        label.text = device.Link.Path;
                    }
                }
            };

            var columnLink = new Column()
            {
                title = "Link",
                stretchable = false
            };

            columnLink.makeCell += MakeCellLink;
            columnLink.bindCell += (element, i) =>
            {
                var item = multiColumnTreeView.GetItemDataForIndex<HierarchyItem>(i);
                if (element is not Toggle toggle) return;
                toggle.SetEnabled(false);
                toggle.style.display = item.Component == null ? DisplayStyle.None : DisplayStyle.Flex;
            };
            
            columnLink.bindCell += BindCellLink;
            columnLink.unbindCell += UnbindCellLink;
            
            multiColumnTreeView.columns.Add(columnHierarchy);
            multiColumnTreeView.columns.Add(columnPath);
            multiColumnTreeView.columns.Add(columnLink);

            multiColumnTreeView.selectionChanged += OnSelectionChanged;
            multiColumnTreeView.itemsChosen += OnItemsChosen;
            
            
            return multiColumnTreeView;
            
            VisualElement MakeCellLink()
            {
                var toggle = new Toggle();
                toggle.SetEnabled(false);
                return toggle;
            }
            
            void BindCellLink(VisualElement visualElement, int i)
            {
                var item = multiColumnTreeView.GetItemDataForIndex<HierarchyItem>(i);
                if (visualElement is not Toggle toggle) return;

                if (item.Component == null)
                {
                    toggle.style.display = DisplayStyle.None;
                    return;
                }
                
                if(item.Component is not IDevice device) return;
                
                toggle.style.display = DisplayStyle.Flex;
                
                toggle.SetValueWithoutNotify(device.Link.Connected.Value);
                toggle.BindProperty(device.Link.Connected);
            }
            
            void UnbindCellLink(VisualElement visualElement, int i)
            {
                if (visualElement is not Toggle toggle) return;
                toggle.UnbindProperty();
            }
        }

        private void OnItemsChosen(IEnumerable<object> obj)
        {
            SceneView.lastActiveSceneView.FrameSelected();
        }

        private void OnSelectionChanged(IEnumerable<object> obj)
        {
            foreach (var element in obj)
            {
                if (element is not HierarchyItem hierarchy) return;
                if (hierarchy.Component == null) return;
                if (hierarchy.Component is not IDevice device) return;
                Selection.SetActiveObjectWithContext(device.Component, device.Component);
            }
        }

        private void RefreshTreeView()
        {
            if (string.IsNullOrEmpty(_searchQuery))
            {
                _multiColumnTreeView.SetRootItems(_treeViewData);
            }
            else
            {
                var filtered = _treeViewData.FilterByName(_searchQuery);
                _multiColumnTreeView.SetRootItems(filtered);
            }
            
            _multiColumnTreeView.RefreshItems();
        }

        private void RefreshTreeViewDataSource()
        {
            _treeViewData = HierarchyFactory.CreateTreeViewData<IDevice>(SceneManager.GetActiveScene(), GetHierarchyLevels);
            RefreshTreeView();
        }
        
        private string[] GetHierarchyLevels(IDevice component)
        {
            component.Link.Initialize(component.Component);
            var path = component.Link.GetHierarchyPath();
            return path.Split('.');
        }
        
        private void OnSearchFilterChanged(ChangeEvent<string> evt)
        {
            ApplySearchFilter(evt.newValue);
        }
        
        private void ApplySearchFilter(string filter)
        {
            _searchQuery = filter;
            RefreshTreeView();
        }

        private void ResetOverride()
        {
            //TODO
            Debug.Log($"Reset Override");
        }
    }
}