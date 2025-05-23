using System;
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

        /// <summary>
        /// Adjusts the GameObject’s BoxCollider to bounding box for all child meshes.
        /// </summary>
        /// <param name="gameObject">Target GameObject.</param>
        /// <param name="boxCollider">The BoxCollider if found and updated; null otherwise.</param>
        /// <returns>True if the collider was found and updated; false on error.</returns>
        public static bool TryBoundBoxColliderSize(GameObject gameObject, out BoxCollider boxCollider)
        {
            try
            {
                if (gameObject == null) throw new NullReferenceException("GameObject is null");
                boxCollider = gameObject.GetComponent<BoxCollider>();
                if (boxCollider == null)
                {
                    throw new NullReferenceException("BoxCollider is null");
                }
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(boxCollider, "Set Box Collider Bound Size");
#endif               
                var bounds = GetLocalBoundsForChildrenMeshes(gameObject);
                boxCollider.center = bounds.center;
                boxCollider.size = bounds.size;
                
#if UNITY_EDITOR
                
                UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"TryBoundBoxColliderSize: {exception}", gameObject);
                boxCollider = null;
                return false;
            }
        }
        
        /// <summary>
        /// Computes the local-space axis-aligned Bounds that encapsulate all MeshFilter meshes in the given GameObject’s hierarchy.
        /// </summary>
        /// <param name="gameObject">Target GameObject.</param>
        /// <returns>Bounds in the GameObject’s local space covering all child meshes.</returns>
        public static Bounds GetLocalBoundsForChildrenMeshes(GameObject gameObject)
        {
            var transform = gameObject.transform;
            var localBounds = new Bounds(Vector3.zero, Vector3.zero);
            var filters = gameObject.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in filters)
            {
                var matrix = transform.localToWorldMatrix.inverse * meshFilter.transform.localToWorldMatrix;
                var axisAlignedBounds = GeometryUtility.CalculateBounds(meshFilter.sharedMesh.vertices, matrix);
                localBounds.Encapsulate(axisAlignedBounds);
            }
            return localBounds;
        }
    }
}
