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
    }
}