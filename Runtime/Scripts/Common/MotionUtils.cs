using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OC.Components;
using UnityEngine;

namespace OC
{
    public static class MotionUtils
    {
        public static IEnumerator Interpolate(
            this Transform transform, 
            Vector3 position, 
            Quaternion rotation,
            float duration,
            TransformSpace space = TransformSpace.Global)
        {
            return space switch
            {
                TransformSpace.Local => Interpolate(transform, transform.localPosition, transform.localRotation, position,
                    rotation, duration, space),
                TransformSpace.Global => Interpolate(transform, transform.position, transform.rotation, position,
                    rotation, duration, space),
                _ => throw new ArgumentOutOfRangeException(nameof(space), space, null)
            };
        }

        public static IEnumerator Interpolate(
            this Transform objectTransform, 
            Vector3 startPosition,
            Quaternion startRotation, 
            Vector3 targetPosition, 
            Quaternion targetRotation, 
            float duration,
            TransformSpace space = TransformSpace.Global)
        {
            if (duration > 0)
            {
                for (var time = 0f; time < duration; time += Time.deltaTime)
                {
                    var progress = time / duration;
                    var position = Vector3.Lerp(startPosition, targetPosition, progress);
                    var rotation = Quaternion.Slerp(startRotation, targetRotation, progress);

                    switch (space)
                    {
                        case TransformSpace.Local:
                            objectTransform.SetLocalPositionAndRotation(position, rotation);
                            break;
                        case TransformSpace.Global:
                            objectTransform.SetPositionAndRotation(position, rotation);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(space), space, null);
                    }

                    yield return null;
                }
            }
            
            switch (space)
            {
                case TransformSpace.Local:
                    objectTransform.SetLocalPositionAndRotation(targetPosition, targetRotation);
                    break;
                case TransformSpace.Global:
                    objectTransform.SetPositionAndRotation(targetPosition, targetRotation);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(space), space, null);
            }
        }

        public static IEnumerator LerpPosition(this Transform objectTransform, Vector3 a, Vector3 b, float duration, TransformSpace space = TransformSpace.Global)
        {
            if (duration > 0)
            {
                for (var time = 0f; time < duration; time += Time.deltaTime)
                {
                    var progress = time / duration;
                    switch (space)
                    {
                        case TransformSpace.Local:
                            objectTransform.localPosition = Vector3.Lerp(a, b, progress);
                            break;
                        case TransformSpace.Global:
                            objectTransform.position = Vector3.Lerp(a, b, progress);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(space), space, null);
                    }  
                    
                    yield return null;
                }
            }
            
            switch (space)
            {
                case TransformSpace.Local:
                    objectTransform.localPosition = b;
                    break;
                case TransformSpace.Global:
                    objectTransform.position = b;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(space), space, null);
            }
        }
        
        public static IEnumerator SlerpRotation(this Transform objectTransform, Quaternion a, Quaternion b, float duration, TransformSpace space = TransformSpace.Global)
        {
            if (duration > 0)
            {
                for (var time = 0f; time < duration; time += Time.deltaTime)
                {
                    var progress = time / duration;
                    switch (space)
                    {
                        case TransformSpace.Local:
                            objectTransform.localRotation = Quaternion.Slerp(a, b, progress);
                            break;
                        case TransformSpace.Global:
                            objectTransform.rotation = Quaternion.Slerp(a, b, progress);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(space), space, null);
                    }
                    
                    yield return null;
                }
            }
            
            switch (space)
            {
                case TransformSpace.Local:
                    objectTransform.localRotation = b;
                    break;
                case TransformSpace.Global:
                    objectTransform.rotation = b;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(space), space, null);
            }
        }

        public static IEnumerator MoveToWithConstSpeed(Transform objectTransform, Transform target, float speed)
        {
            yield return MoveToWithConstSpeed(objectTransform, target.position, target.rotation, speed);
        }

        public static IEnumerator MoveTo(this DrivePosition drive, float target, float speed)
        {
            var start = drive.Target.Value;
            var distance = Mathf.Abs(target - start);
            for (var travaledDistance = 0f; travaledDistance < distance; travaledDistance += speed * Time.deltaTime)
            {
                var t = travaledDistance / distance;
                drive.Target.Value = Mathf.SmoothStep(start, target, t);
                yield return null;
            }
            drive.Target.Value = target;
        }
        
        public static IEnumerator MoveToMax(this Cylinder cylinder)
        {
            cylinder.JogPlus = true;
            cylinder.JogMinus = false;
            yield return new WaitUntil(() => cylinder.OnLimitMax.Value);
        }

        public static IEnumerator MoveToMin(this Cylinder cylinder)
        {
            cylinder.JogPlus = false;
            cylinder.JogMinus = true;
            yield return new WaitUntil(() => cylinder.OnLimitMin.Value);
        }
        
        public static IEnumerator MoveToMax(this List<Cylinder> cylinders)
        {
            foreach (var cylinder in cylinders)
            {
                cylinder.JogPlus = true;
                cylinder.JogMinus = false;
            }

            yield return new WaitUntil(() => cylinders.All(item => item.OnLimitMax.Value));
        }
        
        public static IEnumerator MoveToMin(this List<Cylinder> cylinders)
        {
            foreach (var cylinder in cylinders)
            {
                cylinder.JogPlus = false;
                cylinder.JogMinus = true;
            }

            yield return new WaitUntil(() => cylinders.All(item => item.OnLimitMin.Value));
        }

        public static IEnumerator MoveToWithConstSpeed(Transform objectTransform, Vector3 position, Quaternion rotation, float speed)
        {
            var startPosition = objectTransform.position;
            var startRotation = objectTransform.rotation;
            var targetPosition = position;
            var targetRotation = rotation;

            float distance = Vector3.Distance(objectTransform.position, position);

            for (var travaledDistance = 0f; travaledDistance < distance; travaledDistance += speed * Time.deltaTime)
            {
                var progress = travaledDistance / distance;
                objectTransform.position = Vector3.Lerp(startPosition, targetPosition, progress);
                objectTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, progress);
                yield return null;
            }
            objectTransform.position = targetPosition;
            objectTransform.rotation = targetRotation;
        }
        
        public enum TransformSpace
        {
            Local, 
            Global
        }
    }
}