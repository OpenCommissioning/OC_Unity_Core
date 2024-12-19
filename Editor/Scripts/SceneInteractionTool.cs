using System;
using System.Collections.Generic;
using System.Linq;
using OC.Interactions;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OC
{
    [EditorTool("Scene Interaction")]
    [Icon(ICON)]
    public class SceneInteractionTool : EditorTool
    {
        public const string ICON = "d_EventTrigger Icon";
        
        private readonly RaycastHit[] _hits = new RaycastHit[10];
        private Interaction _activeInteraction;

        [SerializeField]
        private List<Interaction> _hitInteractions = new ();
        [SerializeField]
        private int[] _outlineRenderers;

        private readonly LayerMask _layerMask = 1 << (int)DefaultLayers.Interactions;
        private int _hitsCount;
        
        [Shortcut("Scene Interaction Tool", typeof(SceneView), KeyCode.I)]
        private static void SceneViewInteractionShortcut()
        {
            if (ToolManager.activeToolType == typeof(SceneInteractionTool))
            {
                ToolManager.RestorePreviousTool();
            }
            else
            {
                ToolManager.SetActiveTool(typeof(SceneInteractionTool));
            }
        }
        
        public override void OnActivated()
        {
            SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Scene Interaction Activated"), .1f);
        }
        
        public override void OnWillBeDeactivated()
        {
            SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Scene Interaction Deactivated"), .1f);
            ResetHit();
        }

        public override void OnToolGUI(EditorWindow window)
        {
            if (!Application.isPlaying) return;
            if (window is not SceneView) return;

            var currentEvent = Event.current;

            if (currentEvent.type == EventType.Repaint)
            {
                Handles.DrawOutline(_outlineRenderers, Color.white, 0.05f);
            }
            
            if (currentEvent.type is not (EventType.MouseMove or EventType.MouseUp or EventType.MouseDown)) return;
            if (currentEvent.button != 0) return;

            RayCast(currentEvent);

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
            {
                if (_activeInteraction != null)
                {
                    PointerDownEvent(_activeInteraction);
                }
                else
                {
                    ResetHit();
                }
                currentEvent.Use();
            }

            if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
            {
                if (_activeInteraction != null)
                {
                    PointerClickEvent(_activeInteraction);
                    PointerUpEvent(_activeInteraction);
                }
                currentEvent.Use();
            }
        }
        
        private void RayCast(Event currentEvent)
        {
            Array.Clear(_hits, 0, _hitsCount);
            _hitInteractions.Clear();
                
            var ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            _hitsCount = Physics.RaycastNonAlloc(ray, _hits, 500, _layerMask);

            if (_hitsCount == 0)
            {
                ResetHit();
                return;
            }
                
            var hits = _hits.OrderBy(hit => hit.distance);
            foreach (var raycast in hits)
            {
                if (raycast.distance < Utils.TOLERANCE) continue;
                if (raycast.collider.gameObject.TryGetComponent<Interaction>(out var interaction))
                {
                    _hitInteractions.Add(interaction);
                }
            }
                
            if (_hitInteractions.Count < 1)
            {
                ResetHit();
                return; 
            }
            
            if (_activeInteraction == _hitInteractions.First())
            {
                return;
            }
                
            if (_activeInteraction != null) PointerExitEvent(_activeInteraction);
            _activeInteraction = _hitInteractions.First();
            PointerEnterEvent(_activeInteraction);

            _outlineRenderers = new int[_activeInteraction.Renderers.Count];
            for (var i = 0; i < _activeInteraction.Renderers.Count; i++)
            {
                _outlineRenderers[i] = _activeInteraction.Renderers[i].GetInstanceID();
            }
        }

        private void ResetHit()
        {
            if (_activeInteraction != null)
            {
                PointerUpEvent(_activeInteraction);
                PointerExitEvent(_activeInteraction);
            }
            
            _hitInteractions.Clear();
            Array.Clear(_outlineRenderers, 0, _outlineRenderers.Length);
            _activeInteraction = null;
        }
        
        private void PointerEnterEvent(Interaction interaction)
        {
            ExecuteEvents.Execute(interaction.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
        }
        
        private void PointerExitEvent(Interaction interaction)
        {
            ExecuteEvents.Execute(interaction.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
        }

        private void PointerClickEvent(Interaction interaction)
        {
            ExecuteEvents.Execute(interaction.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        }
        
        private void PointerDownEvent(Interaction interaction)
        {
            ExecuteEvents.Execute(interaction.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
        }
        
        private void PointerUpEvent(Interaction interaction)
        {
            ExecuteEvents.Execute(interaction.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
        }
    }
}
