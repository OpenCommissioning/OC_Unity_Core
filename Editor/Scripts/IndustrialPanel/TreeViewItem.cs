using System.Collections.Generic;

namespace OC.Editor
{
    public readonly struct TreeViewItem<T>
    {
        public string Name { get; }
        public string Path { get; }
        public int Id { get; }
        public List<T> Content => _content;
        public List<TreeViewItem<T>> Children => _children;
        public bool HasChildren => _children != null && _children.Count > 0;
        public bool HasContent => _content != null && _content.Count > 0;

        private readonly List<TreeViewItem<T>> _children;
        private readonly List<T> _content;
        
        public TreeViewItem(string name, string path, int id, List<TreeViewItem<T>> children = null)
        {
            Name = name;
            Path = path;
            Id = id;
            _children = children ?? new List<TreeViewItem<T>>();
            _content = new List<T>();
        }
        
        public void AddChild(TreeViewItem<T> child) => _children.Add(child);
        
        public void AddChildren(IList<TreeViewItem<T>> children)
        {
            foreach (var child in children)
            {
                AddChild(child);
            }
        }
        
        public void InsertChild(TreeViewItem<T> child, int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                _children.Add(child);
            }
            else
            {
                _children.Insert(index, child);
            }
        }
        
        public void RemoveChild(int childId)
        {
            if (_children == null) return;
            for (var index = 0; index < _children.Count; ++index)
            {
                if (childId == _children[index].Id)
                {
                    _children.RemoveAt(index);
                    break;
                }
            }
        }
        
        internal int GetChildIndex(int itemId)
        {
            var childIndex = 0;
            foreach (var child in _children)
            {
                if (child.Id == itemId) return childIndex;
                ++childIndex;
            }
            return -1;
        }
        
        internal void ReplaceChild(TreeViewItem<T> newChild)
        {
            if (!HasChildren) return;
            int index = 0;
            foreach (var child in _children)
            {
                if (child.Id == newChild.Id)
                {
                    _children.RemoveAt(index);
                    _children.Insert(index, newChild);
                    break;
                }
                ++index;
            }
        }
        
    }
}