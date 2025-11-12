using System;
using System.Linq;
using OC.Components;
using UnityEngine;

namespace OC
{
    public static class Math
    {
        public static Vector2[] GetIntersections(Vector2 p1, Vector2 p2, Vector2 center, float radius)
        {
            var dp = p2 - p1;
            var a = Vector2.Dot(dp, dp);
            var b = 2 * Vector2.Dot(dp, p1 - center);
            var c = Vector2.Dot(center, center) - 2 * Vector2.Dot(center, p1) + Vector2.Dot(p1, p1) - radius * radius;
            // ReSharper disable once InconsistentNaming
            var bb4ac = b * b - 4 * a * c;
            if (Mathf.Abs(a) < float.Epsilon || bb4ac < 0)
            {
                //  line does not intersect
                return new [] { p2, p2 };
            }
            var mu1 = (-b + Mathf.Sqrt(bb4ac)) / (2 * a);
            var mu2 = (-b - Mathf.Sqrt(bb4ac)) / (2 * a);
            var sect = new Vector2[2];
            sect[0] = p1 + mu1 * (p2 - p1);
            sect[1] = p1 + mu2 * (p2 - p1);
            return sect;
        }

        public static Vector3 PlaneXYPosition(Vector3 position)
        {
            return new Vector3(position.x, position.y);
        }

        public static Vector3 PlaneXYAngle(Vector3 angles)
        {
            return new Vector3(angles.x, 90f);
        }

        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            var unsignedAngle = Vector3.Angle(from, to);
            var crossX = from.y * to.z - from.z * to.y;
            var crossY = from.z * to.x - from.x * to.z;
            var crossZ = from.x * to.y - from.y * to.x;
            var sign = Mathf.Sign(axis.x * crossX + axis.y * crossY + axis.z * crossZ);
            return unsignedAngle * sign;
        }

        public static Vector2 FindNearestPointOnInfLine(Vector2 start, Vector2 direction, Vector2 point)
        {
            direction.Normalize();
            var lhs = point - start;
            var dotP = Vector2.Dot(lhs, direction);
            return start + direction * dotP;
        }

        public static Vector2 FindNearestPointOnLine(Vector2 start, Vector2 end, Vector2 point)
        {            
            var direction = (end - start);
            var magnitudeMax = direction.magnitude;
            direction.Normalize();
            
            var lhs = point - start;
            var dotP = Vector2.Dot(lhs, direction);
            dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
            return start + direction * dotP;
        }

        public static Vector3 GetClosestPointOnFiniteLine(Vector3 point, Vector3 p0, Vector3 p1)
        {
            var direction = p1 - p0;
            var length = direction.magnitude;
            direction.Normalize();
            var project = Mathf.Clamp(Vector3.Dot(point - p0, direction), 0f, length);
            return p0 + direction * project;
        }

        public static Matrix4x4 GetMatrix(this Transform transform, bool local = false, bool ignoreScale = false)
        {
            if (local)
            {
                var scale = ignoreScale ? Vector3.one : transform.localScale;
                return Matrix4x4.TRS(transform.localPosition, transform.localRotation, scale);
            }
            else
            {
                var scale = ignoreScale ? Vector3.one : transform.lossyScale;
                return Matrix4x4.TRS(transform.position, transform.rotation, scale);
            }
        }
        
        public static void SetMatrix(this Transform transform, Matrix4x4 matrix, bool local = false)
        {
            if (local)
            {
                transform.SetLocalPositionAndRotation(matrix.GetPosition(), matrix.rotation);
            }
            else
            {
                transform.SetPositionAndRotation(matrix.GetPosition(), matrix.rotation);
            }
        }
        
        public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
        {
            var dir = point - pivot;
            dir = Quaternion.Euler(angles) * dir;
            point = dir + pivot;
            return point;
        }
        
        public static Vector3 GetDirection(AxisDirection direction)
        {
            return direction switch
            {
                AxisDirection.X => Vector3.right,
                AxisDirection.Y => Vector3.up,
                AxisDirection.Z => Vector3.forward,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }
        
        public static bool FastApproximately(float a, float b, float threshold = Utils.TOLERANCE)
        {
            return Mathf.Abs(a-b) <= threshold;
        }
        
        public static int Pow(this int bas, int exp)
        {
            return Enumerable
                .Repeat(bas, exp)
                .Aggregate(1, (a, b) => a * b);
        }
        
        public static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var v321 = p3.x * p2.y * p1.z;
            var v231 = p2.x * p3.y * p1.z;
            var v312 = p3.x * p1.y * p2.z;
            var v132 = p1.x * p3.y * p2.z;
            var v213 = p2.x * p1.y * p3.z;
            var v123 = p1.x * p2.y * p3.z;
            return (1.0f / 6.0f) * (-v321 + v231 + v312 - v132 - v213 + v123);
        }

        public static float VolumeOfMesh(Mesh mesh)
        {
            var volume = 0f;
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;

            for (var i = 0; i < triangles.Length; i += 3)
            {
                var p1 = vertices[triangles[i + 0]];
                var p2 = vertices[triangles[i + 1]];
                var p3 = vertices[triangles[i + 2]];
                volume += SignedVolumeOfTriangle(p1, p2, p3);
            }
            return Mathf.Abs(volume);
        }
    }
}
