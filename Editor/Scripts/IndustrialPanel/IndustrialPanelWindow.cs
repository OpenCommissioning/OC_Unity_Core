using System;
using System.Collections.Generic;
using OC.Communication;
using OC.Interactions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        private const string EDITOR_PREFS_LAST_SELECTION = "IndustrialPanel_LastSelection";

        private TwoPaneSplitView _splitView;
        private TreeView _treeView;
        private VisualElement _hierarchyContainer;
        private ScrollView _contentContainer;
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

        private void CreateGUI()
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
            _contentContainer = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            
            hierarchySplitContainer.Add(hierarchyLabel);
            hierarchySplitContainer.Add(_hierarchyEmpty);
            hierarchySplitContainer.Add(_hierarchyContainer);
            
            contentSplitContainer.Add(_contentEmpty);
            contentSplitContainer.Add(_contentContainer);
            
            _splitView = new TwoPaneSplitView(0, 200, TwoPaneSplitViewOrientation.Horizontal);
            _splitView.Add(hierarchySplitContainer);
            _splitView.Add(contentSplitContainer);
            
            _treeView = CreateTreeView();
            _hierarchyContainer.Add(_treeView);
            
            root.Add(_splitView);

            if (EditorPrefs.HasKey(EDITOR_PREFS_SPLIT_VIEW_ORIENTATION))
            {
                _splitViewOrientation = (TwoPaneSplitViewOrientation)EditorPrefs.GetInt(EDITOR_PREFS_SPLIT_VIEW_ORIENTATION);
            }

            _applicationViewMode = Application.isPlaying ? ApplicationView.PlayView : ApplicationView.EditorView;
            
            SetSplitViewOrientation(_splitViewOrientation);
            SetApplicationViewMode(_applicationViewMode);
            RefreshHierarchy();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorPrefs.SetInt(EDITOR_PREFS_SPLIT_VIEW_ORIENTATION, (int)_splitViewOrientation);
            EditorPrefs.SetInt(EDITOR_PREFS_LAST_SELECTION, _treeView.selectedIndex);
        }

        private void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            ApplicationViewMode = state == PlayModeStateChange.EnteredPlayMode ? ApplicationView.PlayView : ApplicationView.EditorView;
        }
        
        private void SetApplicationViewMode(ApplicationView mode)
        {
            switch (mode)
            {
                case ApplicationView.EditorView:
                    _hierarchyEmpty.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    _contentEmpty.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    _hierarchyContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    _contentContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    break;
                case ApplicationView.PlayView:
                    _hierarchyEmpty.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    _contentEmpty.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                    _hierarchyContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    _contentContainer.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            RefreshHierarchy();
        }
        
        private void RefreshHierarchy()
        {
            if (_applicationViewMode != ApplicationView.PlayView) return;
            var treeViewData = HierarchyFactory.CreateTreeViewData<IIndustrialPanel>(SceneManager.GetActiveScene(), GetHierarchyLevels);
            _treeView.SetRootItems(treeViewData);
            _treeView.Rebuild();
            
            if (EditorPrefs.HasKey(EDITOR_PREFS_LAST_SELECTION))
            {
                _treeView.SetSelectionById(EditorPrefs.GetInt(EDITOR_PREFS_LAST_SELECTION));
            }
        }
        
        private string[] GetHierarchyLevels(IIndustrialPanel component)
        {
            component.Link.Initialize(component.Component);
            var path = component.Link.GetHierarchyPath();
            return path.Split('.');
        }

        private void SetSplitViewOrientation(TwoPaneSplitViewOrientation orientation)
        {
            switch (orientation)
            {
                case TwoPaneSplitViewOrientation.Horizontal:
                    _splitView.orientation = TwoPaneSplitViewOrientation.Horizontal;
                    _contentContainer.contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);
                    break;
                case TwoPaneSplitViewOrientation.Vertical:
                    _splitView.orientation = TwoPaneSplitViewOrientation.Vertical;
                    _contentContainer.contentContainer.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null);
            }
        }

        private TreeView CreateTreeView()
        {
            var treeView = new TreeView()
            {
                selectionType = SelectionType.Single,
                horizontalScrollingEnabled = true,
                fixedItemHeight = 18,
                autoExpand = true,
                reorderable = false
            };
            
            treeView.makeItem = MakeItems;
            treeView.bindItem = BindItem;
            treeView.selectionChanged += TreeViewOnSelectionChanged;
            treeView.itemsChosen += TreeViewOnItemChosen;
            
            try
            {
                treeView.SetSelectionById(_lastSelectedId);
            }
            catch (Exception)
            {
                // ignored
            }

            return treeView;

            VisualElement MakeItems() => new Label();

            void BindItem(VisualElement e, int i)
            {
                var item = treeView.GetItemDataForIndex<HierarchyItem>(i);
                if (e is Label label) label.text = item.Name;
            }
        }

        private void TreeViewOnSelectionChanged(IEnumerable<object> obj)
        {
            foreach (var element in obj)
            {
                if (element is not HierarchyItem hierarchy) return;
                if (hierarchy.Component == null) return;
                if (hierarchy.Component is not IIndustrialPanel panel) return;
                Selection.SetActiveObjectWithContext(panel.Component, panel.Component);
                SetContent(panel);
            }
        }

        private void TreeViewOnItemChosen(IEnumerable<object> obj)
        {
            SceneView.lastActiveSceneView.FrameSelected();
        }

        private void SetContent(IIndustrialPanel panel)
        {
            _contentContainer.Clear();
            _contentContainer.Add(panel.Create());
            RefreshBreadcrumbs(panel);
        }

        private void RefreshBreadcrumbs(IIndustrialPanel panel)
        {
            _toolbarBreadcrumbs.Clear();
            var split = GetHierarchyLevels(panel);
            foreach (var item in split)
            {
                _toolbarBreadcrumbs.PushItem(item);
            }
        }

        public enum ApplicationView
        {
            EditorView,
            PlayView
        }
    }
}