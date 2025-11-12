using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace OC.Editor
{
    [EditorTool("Snapping", typeof(SnappingObject))]
    [Icon(ICON)]
    public class SnappingTool : EditorTool
    {
        private const string ICON = "d_Cubemap Icon";
        private const float RADIUS_SNAP = 0.3f;
        private const float RADIUS_SEARCH = 1f;
        private const float SNAP_TANGENT_LENGTH = 1f;
        private List<SnappingPoint> _sceneSnappingPoints;
        private SnappingPoint _nearestSnappingPoint;
        private int _selectedPointIndex;
        
        [Shortcut("Snapping Tool", typeof(SceneView), KeyCode.S)]
        private static void SnappingToolShortcut()
        {
            if (Selection.GetFiltered<SnappingObject>(SelectionMode.TopLevel).Length > 0)
            {
                ToolManager.SetActiveTool(typeof(SnappingTool));
            }
            else
            {
                ToolManager.RestorePreviousTool();
            }
        }

        public override void OnActivated()
        {
            _sceneSnappingPoints = FindObjectsOfType<SnappingPoint>().ToList();
        }

        public override void OnWillBeDeactivated()
        {
            _sceneSnappingPoints?.Clear();
        } 

        public override void OnToolGUI(EditorWindow window)
        {
            var snappingObject = target as SnappingObject;
            if (snappingObject == null) return;
            
            if (GUIUtility.hotControl == 0)
            {
                _selectedPointIndex = -1;
                _nearestSnappingPoint = null;
            }
            
            if (_selectedPointIndex > -1 && _nearestSnappingPoint != null)
            {
                var handleSize = HandleUtility.GetHandleSize(snappingObject.Points[_selectedPointIndex].transform.position);
                var distance = Vector3.Distance(snappingObject.Points[_selectedPointIndex].transform.position, _nearestSnappingPoint.transform.position);
                var scaledDistance = distance * handleSize;

                if (scaledDistance < RADIUS_SNAP)
                {
                    DrawHandleBezier(snappingObject.Points[_selectedPointIndex].transform, _nearestSnappingPoint.transform,  Color.green);
                }
                else if (scaledDistance < RADIUS_SEARCH)
                {
                    DrawHandleBezier(snappingObject.Points[_selectedPointIndex].transform, _nearestSnappingPoint.transform,  Color.white);
                }
                
                var e = Event.current;
                if (e.type == EventType.MouseUp && e.button == 0)
                {
                    if (scaledDistance > RADIUS_SNAP) return;
                    var targetMatrix = Matrix4x4.TRS(
                        _nearestSnappingPoint.transform.position,
                        _nearestSnappingPoint.transform.rotation * Quaternion.AngleAxis(180, Vector3.up),
                        Vector3.one);
                    SetSnappingObjectMatrix(snappingObject, snappingObject.Points[_selectedPointIndex], targetMatrix);
                    e.Use();
                    
                    //Reset state
                    _selectedPointIndex = -1;
                    _nearestSnappingPoint = null;
                    GUIUtility.hotControl = 0; 
                }
            }
            
            for (var i = 0; i < snappingObject.Points.Count; i++)
            {
                var point = snappingObject.Points[i];
                if (point.SnapType is SnappingPoint.Type.Slot or SnappingPoint.Type.None) continue;
                    
                EditorGUI.BeginChangeCheck();
                var targetPosition = Handles.PositionHandle(point.transform.position, point.transform.rotation);
                var targetMatrix = Matrix4x4.TRS(targetPosition, point.transform.rotation, Vector3.one);
                    
                if (EditorGUI.EndChangeCheck())
                {
                    _selectedPointIndex = i;
                    if (TryFindNearestSnappingPoint(point, RADIUS_SEARCH * HandleUtility.GetHandleSize(point.transform.position), out var snappingPoint))
                    {
                        _nearestSnappingPoint = snappingPoint;
                    }
                    
                    SetSnappingObjectMatrix(snappingObject, point, targetMatrix);
                }
            }
        }

        private void DrawHandleBezier(Transform from, Transform to, Color color)
        {
            var distance = Vector3.Distance(from.position, to.position);
            var tangent = distance * 0.3f * SNAP_TANGENT_LENGTH;
            var fromTangent = from.position + from.forward * tangent;
            var toTangent = to.position + to.forward * tangent;
            Handles.DrawBezier(from.position, to.position, fromTangent, toTangent, color, null, 5);
        }

        private bool TryFindNearestSnappingPoint(SnappingPoint dragged, float radius, out SnappingPoint nearestSnappingPoint)
        {
            var distance = radius;
            nearestSnappingPoint = null;
            
            foreach (var snappingPoint in _sceneSnappingPoints)
            {
                if (snappingPoint.Parent == dragged.Parent) continue;
                if (snappingPoint.GroupId != dragged.GroupId) continue;
                if (snappingPoint.SnapType == SnappingPoint.Type.Plug) continue;
                var d = Vector3.Distance(snappingPoint.transform.position, dragged.transform.position);
                if (!(d < distance)) continue;
                distance = d;
                nearestSnappingPoint = snappingPoint;
            }
            
            return nearestSnappingPoint != null;
        }
        
        private void SetSnappingObjectMatrix(SnappingObject snappingObject, SnappingPoint point, Matrix4x4 worldMatrix)
        {
            if (snappingObject == null) return;
            var root = snappingObject.transform.GetMatrix(ignoreScale: true);
            var offset = point.transform.GetMatrix(ignoreScale: true).inverse * root;
            snappingObject.transform.SetMatrix(worldMatrix * offset);
            Undo.RecordObject(snappingObject.transform, "Move SnappingObject");
        }
    }
}