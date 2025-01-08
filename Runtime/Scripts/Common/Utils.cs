using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace OC
{
    public static class Utils
    {
        public const float TOLERANCE = 1e-6f;
        public const float TOLERANCE_HALF = 1e-3f;

        public static void DeleteAllChildren(this Transform transfrom, bool immediate = false)
        {
            foreach (Transform child in transfrom)
            {
                if (immediate)
                {
                    Object.DestroyImmediate(child.gameObject);
                }
                else
                {
                    Object.Destroy(child.gameObject);
                }
            }
        }
        
        public static string GetPath(this Transform transform)
        {
            var path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }
        
        public static void DestroyChildrenImmediate(this GameObject t) 
        {
            t.transform.Cast<Transform>().ToList().ForEach(c => Object.DestroyImmediate(c.gameObject));
        }

        public static bool IsInRange(double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        public static bool IsInRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        public static void RemoveNull<T>(List<T> list)
        {
            list?.RemoveAll(item => item == null);
        }

        public static IEnumerator DestroyAtEndFrame(GameObject gameObject)
        {
            yield return new WaitForEndOfFrame();
            Object.DestroyImmediate(gameObject);
        }

        public static bool IsPointerOverUIElement()
        {
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Any(r => r.gameObject.layer == 5);
        }

        public static Color ScaleRGB(this Color color, float factor)
        {
            return new Color(color.r * factor, color.g * factor, color.b * factor, color.a);
        }

        public static void Destroy(this Object obj)
        {
            if (obj == null)
            {
                return;
            }
 
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(obj);
            }
            else
#endif
            {
                Object.Destroy(obj);
            }
        }
        
        public static Bounds GetLocalBoundsForChildrenMeshes(GameObject go)
        {
            var referenceTransform = go.transform;
            var b = new Bounds(Vector3.zero, Vector3.zero);
            RecurseEncapsulate(referenceTransform, ref b);
            return b;
                       
            void RecurseEncapsulate(Transform child, ref Bounds bounds)
            {
                var mesh = child.GetComponent<MeshFilter>();
                if (mesh)
                {
                    var lsBounds = mesh.sharedMesh.bounds;
                    var wsMin = child.TransformPoint(lsBounds.center - lsBounds.extents);
                    var wsMax = child.TransformPoint(lsBounds.center + lsBounds.extents);
                    bounds.Encapsulate(referenceTransform.InverseTransformPoint(wsMin));
                    bounds.Encapsulate(referenceTransform.InverseTransformPoint(wsMax));
                }
                foreach (Transform grandChild in child.transform)
                {
                    RecurseEncapsulate(grandChild, ref bounds);
                }
            }
        }
    }
}
