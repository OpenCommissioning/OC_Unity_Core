using System.Collections.Generic;
using System.Linq;
using OC.SnapingTool;
using UnityEditor;
using UnityEngine;

namespace OC.SnappingTool
{
    [CustomEditor(typeof(SnappingHandles)), CanEditMultipleObjects]
    public class SnappingHandlesEditor : UnityEditor.Editor
    {
        
        private SnapPoint[] _allPoints;
        private bool _hotkey;
        private bool _focused;

        private void OnSceneGUI()
        {
            HandleHotkeyEvent();
            var handle = (SnappingHandles)target;

            if (!_hotkey) return;
            foreach (var snapPoint in handle.GetComponentsInChildren<SnapPoint>())
            {
                if (snapPoint.SnapType is SnapPoint.Type.Slot or SnapPoint.Type.None) continue;
                var newTargetPosition =Handles.PositionHandle(snapPoint.transform.position, snapPoint.transform.rotation);
                if (snapPoint.transform.position == newTargetPosition) continue;

                Undo.RecordObject(snapPoint.Parent.transform, "Move SnapPoint Parent");
                Undo.RecordObject(snapPoint.transform, "Move SnapPoint");

                _allPoints = FindObjectsOfType<SnapPoint>();
                var handleSize = HandleUtility.GetHandleSize(newTargetPosition);
                var targetPoint = GetSnapPoint(newTargetPosition, snapPoint, _allPoints.ToList(), handleSize);

                if (targetPoint)
                {
                    snapPoint.SetGlobalMatrix(targetPoint.transform.GetMatrix() * Matrix4x4.TRS(Vector3.zero,
                        Quaternion.AngleAxis(180, Vector3.up), Vector3.one));
                }
                else
                {
                    var matrix = Matrix4x4.TRS(newTargetPosition, snapPoint.transform.rotation, Vector3.one);
                    snapPoint.SetGlobalMatrix(matrix);
                }
            }
        }

        private static SnapPoint GetSnapPoint(Vector3 position, SnapPoint dragged, List<SnapPoint> points, float radius)
        {
            var distance = radius;
            var index = -1;

            for (var i = 0; i < points.Count; i++)
            {
                if (points[i].Parent == dragged.Parent) continue;
                if (points[i].GroupId != dragged.GroupId) continue;
                if (points[i].SnapType == SnapPoint.Type.Plug) continue;
                var dist = Vector3.Distance(position, points[i].transform.position);
                if (dist < distance)
                {
                    distance = dist;
                    index = i;
                }
                else continue;
            }

            return index < 0 ? null : points[index];
        }

        private void HandleHotkeyEvent()
        {
            var e = Event.current;
            if (e.keyCode == KeyCode.S)
            {
                if (e.type == EventType.KeyDown) _hotkey = Tools.hidden = true;
                else if (e.type == EventType.KeyUp) _hotkey = Tools.hidden = false;
            }
        }
    }
}