using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        private List<TreeViewItemData<HierarchyItem>> _displayedTreeViewData = new ();
        private ToolbarSearchField _toolbarSearchField;
        private ToolbarButton _toolbarButtonRefresh;
        private ToolbarButton _toolbarButtonExport;
        private ToolbarButton _toolbarButtonReset;
        private ToolbarSearchField _filterPath;
        private ToolbarSearchField _filterType;
        private DropdownField _filterLink;
        private DropdownField _filterOverride;
        private string _searchQuery = string.Empty;
        private string _pathQuery = string.Empty;
        private string _typeQuery = string.Empty;
        private StateFilter _linkFilter = StateFilter.All;
        private StateFilter _overrideFilter = StateFilter.All;

        private enum StateFilter
        {
            All,
            On,
            Off
        }
        
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
            _toolbarButtonExport = rootVisualElement.Q<ToolbarButton>("toolbarButtonExport");
            _toolbarButtonReset = rootVisualElement.Q<ToolbarButton>("toolbarButtonReset");
            _filterPath = rootVisualElement.Q<ToolbarSearchField>("filterPath");
            _filterType = rootVisualElement.Q<ToolbarSearchField>("filterType");
            _filterLink = rootVisualElement.Q<DropdownField>("filterLink");
            _filterOverride = rootVisualElement.Q<DropdownField>("filterOverride");

            _filterLink.choices = new List<string> { "All", "Connected", "Disconnected" };
            _filterLink.index = 0;
            _filterOverride.choices = new List<string> { "All", "Active", "Inactive" };
            _filterOverride.index = 0;

            _toolbarSearchField.RegisterValueChangedCallback(OnSearchFilterChanged);
            _toolbarButtonRefresh.clicked += RefreshTreeViewDataSource;
            _toolbarButtonExport.clicked += ExportCsv;
            _toolbarButtonReset.clicked += ResetOverride;
            _filterPath.RegisterValueChangedCallback(OnPathFilterChanged);
            _filterType.RegisterValueChangedCallback(OnTypeFilterChanged);
            _filterLink.RegisterValueChangedCallback(OnLinkFilterChanged);
            _filterOverride.RegisterValueChangedCallback(OnOverrideFilterChanged);
            
            var content = rootVisualElement.Q("content");
            _multiColumnTreeView = CreateMultiColumnTreeView();
            content.Add(_multiColumnTreeView);

            RefreshTreeViewDataSource();
        }
        
        private void OnDisable()
        {
            _toolbarSearchField?.UnregisterValueChangedCallback(OnSearchFilterChanged);
            if (_toolbarButtonRefresh != null) _toolbarButtonRefresh.clicked -= RefreshTreeViewDataSource;
            if (_toolbarButtonExport != null) _toolbarButtonExport.clicked -= ExportCsv;
            if (_toolbarButtonReset != null) _toolbarButtonReset.clicked -= ResetOverride;
            _filterPath?.UnregisterValueChangedCallback(OnPathFilterChanged);
            _filterType?.UnregisterValueChangedCallback(OnTypeFilterChanged);
            _filterLink?.UnregisterValueChangedCallback(OnLinkFilterChanged);
            _filterOverride?.UnregisterValueChangedCallback(OnOverrideFilterChanged);
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
                        label.text = device.Link.ScenePath;
                    }
                }
            };

            var columnType = new Column()
            {
                title = "Type",
                stretchable = true,
                minWidth = 80f
            };
            columnType.bindCell += (element, i) =>
            {
                var item = multiColumnTreeView.GetItemDataForIndex<HierarchyItem>(i);
                if (element is not Label label) return;

                if (item.Component == null)
                {
                    label.style.display = DisplayStyle.None;
                    return;
                }

                label.style.display = DisplayStyle.Flex;
                label.text = item.Component.GetType().Name;
            };

            var columnLink = new Column()
            {
                title = "Link",
                stretchable = false
            };

            columnLink.makeCell += MakeCellLink;
            columnLink.bindCell += BindCellLink;
            columnLink.unbindCell += UnbindCellLink;
            
            var columnOverride = new Column()
            {
                title = "Override",
                stretchable = false
            };
            columnOverride.makeCell += MakeCellOverride;
            columnOverride.bindCell += BindCellOverride;
            columnOverride.unbindCell += UnbindCellOverride;
            
            multiColumnTreeView.columns.Add(columnHierarchy);
            multiColumnTreeView.columns.Add(columnPath);
            multiColumnTreeView.columns.Add(columnType);
            multiColumnTreeView.columns.Add(columnLink);
            multiColumnTreeView.columns.Add(columnOverride);

            multiColumnTreeView.selectionChanged += OnSelectionChanged;
            multiColumnTreeView.itemsChosen += OnItemsChosen;
            
            
            return multiColumnTreeView;
            
            VisualElement MakeCellLink()
            {
                var toggle = new Toggle();
                toggle.SetEnabled(false);
                return toggle;
            }
            
            VisualElement MakeCellOverride()
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
            
            void BindCellOverride(VisualElement visualElement, int i)
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
                
                toggle.SetValueWithoutNotify(device.Override.Value);
                toggle.BindProperty(device.Override);
            }
            
            void UnbindCellOverride(VisualElement visualElement, int i)
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
            _displayedTreeViewData = HasActiveFilter
                ? _treeViewData.Filter(MatchesFilters)
                : _treeViewData;

            _multiColumnTreeView.SetRootItems(_displayedTreeViewData);
            _multiColumnTreeView.RefreshItems();
        }

        private bool HasActiveFilter =>
            !string.IsNullOrEmpty(_searchQuery)
            || !string.IsNullOrEmpty(_pathQuery)
            || !string.IsNullOrEmpty(_typeQuery)
            || _linkFilter != StateFilter.All
            || _overrideFilter != StateFilter.All;

        private bool MatchesFilters(HierarchyItem item)
        {
            if (item.Component == null) return false;

            var device = item.Component as IDevice;

            if (!string.IsNullOrEmpty(_searchQuery) && !MatchesSearchQuery(item, device)) return false;
            if (!string.IsNullOrEmpty(_pathQuery) && !Contains(device?.Link.ScenePath, _pathQuery)) return false;
            if (!string.IsNullOrEmpty(_typeQuery) && !Contains(item.Component.GetType().Name, _typeQuery)) return false;
            if (!MatchesState(_linkFilter, device?.Link.Connected.Value)) return false;
            if (!MatchesState(_overrideFilter, device?.Override.Value)) return false;

            return true;
        }

        private bool MatchesSearchQuery(HierarchyItem item, IDevice device)
        {
            return Contains(item.Name, _searchQuery)
                   || Contains(item.Component.GetType().Name, _searchQuery)
                   || Contains(device?.Link.ScenePath, _searchQuery);
        }

        private static bool MatchesState(StateFilter filter, bool? state)
        {
            if (filter == StateFilter.All) return true;
            return state == (filter == StateFilter.On);
        }

        private static bool Contains(string value, string query)
        {
            return value?.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
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
            _searchQuery = Sanitize(filter);
            RefreshTreeView();
        }

        private void OnPathFilterChanged(ChangeEvent<string> evt)
        {
            _pathQuery = Sanitize(evt.newValue);
            RefreshTreeView();
        }

        private void OnTypeFilterChanged(ChangeEvent<string> evt)
        {
            _typeQuery = Sanitize(evt.newValue);
            RefreshTreeView();
        }

        private void OnLinkFilterChanged(ChangeEvent<string> evt)
        {
            _linkFilter = (StateFilter)_filterLink.index;
            RefreshTreeView();
        }

        private void OnOverrideFilterChanged(ChangeEvent<string> evt)
        {
            _overrideFilter = (StateFilter)_filterOverride.index;
            RefreshTreeView();
        }

        private static string Sanitize(string filter)
        {
            return filter?.Trim() ?? string.Empty;
        }

        private void ExportCsv()
        {
            var path = UnityEditor.EditorUtility.SaveFilePanel(
                "Export Project Inspector",
                string.Empty,
                $"ProjectInspector_{SceneManager.GetActiveScene().name}",
                "csv");

            if (string.IsNullOrEmpty(path)) return;

            var csv = new StringBuilder();
            csv.AppendLine("Hierarchy,Path,Type,Link,Override");

            foreach (var rootItem in _displayedTreeViewData)
            {
                AppendCsvRows(csv, rootItem, string.Empty);
            }

            File.WriteAllText(path, csv.ToString(), Encoding.UTF8);
        }

        private static void AppendCsvRows(
            StringBuilder csv,
            TreeViewItemData<HierarchyItem> treeItem,
            string parentHierarchy)
        {
            var item = treeItem.data;
            var hierarchy = string.IsNullOrEmpty(parentHierarchy)
                ? item.Name
                : $"{parentHierarchy}.{item.Name}";

            if (item.Component is IDevice device)
            {
                csv.Append(EscapeCsv(hierarchy)).Append(',');
                csv.Append(EscapeCsv(device.Link.ScenePath)).Append(',');
                csv.Append(EscapeCsv(item.Component.GetType().Name)).Append(',');
                csv.Append(device.Link.Connected.Value).Append(',');
                csv.Append(device.Override.Value).AppendLine();
            }

            foreach (var child in treeItem.children)
            {
                AppendCsvRows(csv, child, hierarchy);
            }
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0) return value;
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }

        private void ResetOverride()
        {
#if UNITY_6000_3_OR_NEWER
            var devices = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID).OfType<IDevice>().ToList();
#else
            var devices = FindObjectsOfType<MonoBehaviour>().OfType<IDevice>().ToList();
#endif
            foreach (var item in devices.Where(item => item.Override.Value))
            {
                item.Override.Value = false;
                Logging.Logger.Log(LogType.Warning, $"Component Override is DISABLED: {item.Link.ScenePath}");
            }
        }
    }
}