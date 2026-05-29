using UnityEngine;

namespace OC
{
    public static class TransformExtensions
    {
        public static T GetComponentInParent<T>(this Transform transform, int maxDepth) where T : Component
        {
            var current = transform;
            var depth = 0;

            while (current != null && depth <= maxDepth)
            {
                var component = current.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }

                current = current.parent;
                depth++;
            }

            return null;
        }
        
        public static string GetScenePath(this Transform target)
        {
            if (target == null) return string.Empty;

            var path = target.name;
            var current = target.parent;

            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }
        
        public static string GetScenePath(this Component component)
        {
            var target = component.transform;
            return GetScenePath(target);
        }
    }
}