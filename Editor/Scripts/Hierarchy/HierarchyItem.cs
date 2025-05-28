using System.Collections.Generic;
using UnityEngine;

namespace OC.Editor
{
    public class HierarchyItem
    {
        public string Name { get; private set; }
        public Component Component { get; private set; }
        public List<HierarchyItem> Children { get; private set; }
        public bool HasChildren => Children is { Count: > 0 };

        public HierarchyItem(string name, Component component, List<HierarchyItem> children = null)
        {
            Name = name;
            Component = component;
            Children = children ?? new List<HierarchyItem>();
        }
    }
}