using System;
using System.Collections.Generic;
using System.Linq;
using OC.Interactions;
using OC.Project;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Editor.IndustrialPanel
{ 
    public class IndustrialPanelWindow : EditorWindow
    {
        public ApplicationView ApplicationViewMode
        {
            get => _applicationViewMode;
            set
            {
                if (_applicationViewMode == value) return;
                _applicationViewMode = value;
                SetApplicationViewMode(value);
            }
        }

        public TwoPaneSplitViewOrientation SplitViewOrientation
        {
            get => _splitViewOrientation;
            set
            {
                if (_splitViewOrientation == value) return;
                _splitViewOrientation = value;
                SetSplitViewOrientation(value);
            }
        }

        private ApplicationView _applicationViewMode;
        private TwoPaneSplitViewOrientation _splitViewOrientation;

        private bool _playMode;
        private bool _isTreeViewMode;
        private bool _isContentVertical;

        private const string EDITOR_PREFS_SPLIT_VIEW_ORIENTATION = "IndustrialPanel_SplitViewOrientation";

        private TwoPaneSplitView _splitView;
        private VisualElement _hierarchyContainer;
        private ScrollView _contentContiner;
        private VisualElement _hierarchyEmpty;
        private VisualElement _contentEmpty;
        private ToolbarBreadcrumbs _toolbarBreadcrumbs;
        private int _lastSelectedId;

        [MenuItem("Open Commissioning/Industrial Panel")]
        public static void ShowWindow()
        {
            var window = GetWindow<IndustrialPanelWindow>();
            window.titleContent = new GUIContent("Industrial Panel");
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;
            var toolbar = new Toolbar();
            var toolbarMenu = new ToolbarMenu
            {
                text = "View"
            };

            toolbarMenu.menu.AppendAction("Horizontal View",_ => SplitViewOrientation = TwoPaneSplitViewOrientation.Horizontal, _ => DropdownMenuAction.Status.Normal);
            toolbarMenu.menu.AppendAction("Vertical View",_ => SplitViewOrientation = TwoPaneSplitViewOrientation.Vertical, _ => DropdownMenuAction.Status.Normal);
            toolbarMenu.Add(new ToolbarSpacer());
            _toolbarBreadcrumbs = new ToolbarBreadcrumbs();
            toolbarMenu.Add(_toolbarBreadcrumbs);
            

            toolbar.Add(toolbarMenu);
            root.Add(toolbar);

            var hierarchyLabel = new Label("Hierarchy")
            {
                style =
                {
                    backgroundColor = new Color(0.15f, 0.15f, 0.15f),
                    alignContent = new StyleEnum<Align>(Align.Center),
                    alignItems = new StyleEnum<Align>(Align.Center),
                    color = Color.gray,
                    unityFont = new StyleFont((StyleKeyword)FontStyle.Bold),
                    unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleCenter)
                }
            };

            _hierarchyEmpty = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    alignContent = new StyleEnum<Align>(Align.Center),
                    alignItems = new StyleEnum<Align>(Align.Center),
                    justifyContent = new StyleEnum<Justify>(Justify.Center),
                    color = Color.gray
                }
            };
            _hierarchyEmpty.Add(new Label("Empty"));
            
            var hierarchySplitContainer = new VisualElement();
            var contentSplitContainer = new VisualElement();
            
            _contentEmpty = new VisualElement
            {
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    alignContent = new StyleEnum<Align>(Align.Center),
                    alignItems = new StyleEnum<Align>(Align.Center),
                    justifyContent = new StyleEnum<Justify>(Justify.Center),
                    color = Color.gray
                }
            };
            _contentEmpty.Add(new Label("Scene isn't started"));

            _hierarchyContainer = new VisualElement();
            _contentContiner = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            
            hierarchySplitContainer.Add(hierarchyLabel);
            hierarchySplitContainer.Add(_hierarchyEmpty);
            hierarchySplitContainer.Add(_hierarchyContainer);
            
            contentSplitContainer.Add(_contentEmpty);
            contentSplitContainer.Add(_contentContiner);
            
            _splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);
            _splitView.Add(hierarchySplitContainer);
            _splitView.Add(contentSplitContainer);
            
            root.Add(_splitView);

            if (EditorPrefs.HasKey(EDITOR_PREFS_SPLIT_VIEW_ORIENTATION))
            {
                _splitViewOrientation = (TwoPaneSplitViewOrientation)EditorPrefs.GetInt(EDITOR_PREFS_SPLIT_VIEW_ORIENTATION);
            }

            _applicationViewMode = Application.isPlaying ? ApplicationView.PlayView : ApplicationView.EditorView;

            Rebuild();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorPrefs.SetInt(EDITOR_PREFS_SPLIT_VIEW_ORIENTATION, (int)_splitViewOrientation);
        }

        private void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            ApplicationViewMode = state == PlayModeStateChange.EnteredPlayMode ? ApplicationView.PlayView : ApplicationView.EditorView;
        }

        private void Rebuild()
        {
            SetSplitViewOrientation(_splitViewOrientation);
            SetApplicationViewMode(_applicationViewMode);
        }
        
        private void SetApplicationViewMode(ApplicationView mode)
        {
            switch (mode)
            {
                case ApplicationView.EditorView:
                    _hierarchyEmpty.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    _contentEmpty.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    _hierarchyContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    _contentContiner.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    break;
                case ApplicationView.PlayView:
                    _hierarchyEmpty.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    _contentEmpty.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    _hierarchyContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    _contentContiner.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            RefreshHierarchy();
        }
        
        private void RefreshHierarchy()
        {
            _hierarchyContainer.Clear();
            _contentContiner.Clear();

            if (_applicationViewMode != ApplicationView.PlayView) return;
            
            var panels = IndustrialPanelManager.Instance.IndustrialPanels;
            _hierarchyContainer.Add(CreateTreeHierarchy(panels));
        }

        private void SetSplitViewOrientation(TwoPaneSplitViewOrientation orientation)
        {
            switch (orientation)
            {
                case TwoPaneSplitViewOrientation.Horizontal:
                    _splitView.orientation = TwoPaneSplitViewOrientation.Horizontal;
                    _contentContiner.contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    break;
                case TwoPaneSplitViewOrientation.Vertical:
                    _splitView.orientation = TwoPaneSplitViewOrientation.Vertical;
                    _contentContiner.contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        private VisualElement CreateTreeHierarchy(IReadOnlyList<IIndustrialPanel> panels)
        {
            var treeView = new TreeView();
            var treeViewData = GetTreeData(panels);

            VisualElement MakeItems() => new Label();

            void BindItem(VisualElement e, int i)
            {
                var item = treeView.GetItemDataForIndex<TreeViewItem<IIndustrialPanel>>(i);
                if (e is Label label) label.text = item.Name;
            }

            treeView.SetRootItems(treeViewData);
            treeView.makeItem = MakeItems;
            treeView.bindItem = BindItem;
            treeView.selectionType = SelectionType.Single;
            treeView.Rebuild();
            
            treeView.selectionChanged += TreeViewOnselectionChanged;

            treeView.horizontalScrollingEnabled = true;
            treeView.fixedItemHeight = 16;

            try
            {
                treeView.SetSelectionById(_lastSelectedId);
            }
            catch (Exception)
            {
                // ignored
            }

            return treeView;
        }

        private void TreeViewOnselectionChanged(IEnumerable<object> obj)
        {
            foreach (var item in obj)
            {
                var treeItem = item is TreeViewItem<IIndustrialPanel> viewItem ? viewItem : default;
                if (!treeItem.HasContent) continue;
                _lastSelectedId = treeItem.Id;
                CreateContent(treeItem.Content);
                RefreshBreadcrumbs(treeItem.Path);
            }
        }

        private void CreateContent(List<IIndustrialPanel> panels)
        {
            _contentContiner.Clear();
            foreach (var panel in panels)
            {
                _contentContiner.Add(panel.Create());
            }
        }

        private void RefreshBreadcrumbs(string path)
        {
            _toolbarBreadcrumbs.Clear();
            var split = path.Split(".");
            foreach (var item in split)
            {
                _toolbarBreadcrumbs.PushItem(item);
            }
        }

        private List<TreeViewItemData<TreeViewItem<IIndustrialPanel>>> GetTreeData(IReadOnlyList<IIndustrialPanel> panels)
        {
            var count = 0;
            var root = new TreeViewItem<IIndustrialPanel>("root", "root", count);

            foreach (var panel in panels)
            {
                var split = panel.Link.Path.Split(".");
                split[0] += $" ({panel.Link.Client.name})";
                
                var current = root;
                
                for (var i = 0; i < split.Length-1; i++)
                {
                    TreeViewItem<IIndustrialPanel> item;
                    var index = current.Children.FindIndex(x => x.Name == split[i]);
                    if (index < 0)
                    {
                        count++;
                        item = new TreeViewItem<IIndustrialPanel>(split[i], string.Join('.', split.Take(i+1)), count);
                        current.AddChild(item);
                        count++;
                    }
                    else
                    {
                        item = current.Children[index];
                    }

                    current = item;
                }
                
                current.Content.Add(panel);
            }
            
            var result = new List<TreeViewItemData<TreeViewItem<IIndustrialPanel>>>(count);

            foreach (var child in root.Children)
            {
                result.Add(GetViewItem(child));
            }

            return result;
        }

        private TreeViewItemData<TreeViewItem<IIndustrialPanel>> GetViewItem(TreeViewItem<IIndustrialPanel> item)
        {
            var children = new List<TreeViewItemData<TreeViewItem<IIndustrialPanel>>>();
            
            if (item.HasChildren)
            {
                foreach (var child in item.Children)
                {
                    children.Add(GetViewItem(child));
                }
            }

            return new TreeViewItemData<TreeViewItem<IIndustrialPanel>>(item.Id, item, children);
        }

        public enum ApplicationView
        {
            EditorView,
            PlayView
        }
    }
}