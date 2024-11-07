using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace OC
{
    public class Snapping : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _axis;
        [SerializeField] 
        private SnapType _snapType;
        [SerializeField]
        private float _duration;

        public void Snap(GameObject child)
        {
            StopAllCoroutines();
            StartCoroutine(SnapToLocalNull(child));
        }

        [Button]
        public void SnapChildren()
        {
            StopAllCoroutines();
            foreach (Transform child in transform)
            {
                if (!child.gameObject.activeInHierarchy) continue;
                StartCoroutine(SnapToLocalNull(child.gameObject));
            }
        }

        private IEnumerator SnapToLocalNull(GameObject target)
        {
            var position = GetTarget(target.transform.localPosition, _axis);
            var rotation = GetTarget(target.transform.localRotation.eulerAngles, _axis);

            yield return _snapType switch
            {
                SnapType.Position => target.transform.LerpPosition(target.transform.localPosition, position, _duration, MotionUtils.TransformSpace.Local),
                SnapType.Rotation => target.transform.SlerpRotation(target.transform.localRotation, Quaternion.Euler(rotation), _duration, MotionUtils.TransformSpace.Local),
                SnapType.All => target.transform.Interpolate(target.transform.localPosition, target.transform.localRotation, position, Quaternion.Euler(rotation), _duration, MotionUtils.TransformSpace.Local),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private Vector3 GetTarget(Vector3 target, Vector3 axis)
        {
            var result = target;
            if (axis.x != 0) result.x = 0;
            if (axis.y != 0) result.y = 0;
            if (axis.z != 0) result.z = 0;
            return result;
        }

        private enum SnapType
        {
            Position,
            Rotation,
            All
        }
    }
}
