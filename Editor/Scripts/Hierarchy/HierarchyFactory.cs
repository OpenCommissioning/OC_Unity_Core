using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace OC.Editor
{
    public static class HierarchyFactory
    {
        public static List<TreeViewItemData<HierarchyItem>> CreateTreeViewData<T>(Scene scene, Func<T, string[]> getHierarchyLevels) where T : IComponent
        {
            var components = new List<T>();

            foreach (var rootGameObject in scene.GetRootGameObjects())
            {
                components.AddRange(rootGameObject.GetComponentsInChildren<T>());
            }
            
            var root = new HierarchyItem(scene.name, null);
            
            foreach (var component in components)
            {
                var level = getHierarchyLevels(component);

                var parent = root;
                
                for (var i = 0; i < level.Length - 1; i++)
                {
                    HierarchyItem view = null;
                    if (parent.HasChildren)
                    {
                        foreach (var child in parent.Children)
                        {
                            if (!string.Equals(child.Name, level[i])) continue;
                            view = child;
                            break;
                        }
                    }

                    if (view == null)
                    {
                        view = new HierarchyItem(level[i], null);
                        parent.Children.Add(view);
                    }
                    
                    parent = view;
                }

                parent.Children.Add(new HierarchyItem(component.Component.name, component.Component));
            }

            return ResampleTreeViewData(root);
        }

        public static List<TreeViewItemData<HierarchyItem>> FilterByName(this List<TreeViewItemData<HierarchyItem>> source, string name)
        {
            var root = new HierarchyItem("root", null);

            foreach (var item in source)
            {
                root.Children.AddRange(GetChildrenByFilteredName(item.data, name));
            }
            
            return ResampleTreeViewData(root);
        }
        
        private static List<TreeViewItemData<HierarchyItem>> ResampleTreeViewData(HierarchyItem treeViewData)
        {
            var result = new List<TreeViewItemData<HierarchyItem>>();
            var count = 0;

            foreach (var rootChild in treeViewData.Children)
            {
                result.Add(CreateTreeItem(rootChild, ref count));
            }
            
            return result;
        }

        private static TreeViewItemData<HierarchyItem> CreateTreeItem(HierarchyItem item, ref int count)
        {
            if (item.HasChildren)
            {
                var itemDataIndex = count++;
                
                var children = new List<TreeViewItemData<HierarchyItem>>();
                foreach (var child in item.Children)
                {
                    children.Add(CreateTreeItem(child, ref count));
                }
                return new TreeViewItemData<HierarchyItem>(itemDataIndex, item, children);
            }
            else
            {
                return new TreeViewItemData<HierarchyItem>(count++, item);
            }
        }

        private static List<HierarchyItem> GetChildrenByFilteredName(HierarchyItem source, string name)
        {
            var result = new List<HierarchyItem>();

            if (source.HasChildren)
            {
                foreach (var child in source.Children)
                {
                    result.AddRange(GetChildrenByFilteredName(child, name));
                }
            }
            else
            {
                if (source.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) result.Add(source);
            }

            return result;
        }
    }
}