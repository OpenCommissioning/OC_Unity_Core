using UnityEditor;
using UnityEngine;

namespace OC.Editor
{
    public class GizmosUtils
    {
        public static void DrawHandle(Matrix4x4 frame, float scale = 0.1f)
        {
            var position = frame.GetPosition();
            var rotation = frame.rotation;
            
            Handles.color = Handles.xAxisColor;
            Handles.ArrowHandleCap(
                0,
                position,
                rotation * Quaternion.LookRotation(Vector3.right),
                scale,
                EventType.Repaint
            );
            Handles.color = Handles.yAxisColor;
            Handles.ArrowHandleCap(
                0,
                position,
                rotation * Quaternion.LookRotation(Vector3.up),
                scale,
                EventType.Repaint
            );
            Handles.color = Handles.zAxisColor;
            Handles.ArrowHandleCap(
                0,
                position,
                rotation * Quaternion.LookRotation(Vector3.forward),
                scale,
                EventType.Repaint
            );
        }
    }
}