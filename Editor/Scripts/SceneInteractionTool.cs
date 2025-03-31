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
        private readonly LayerMask _layerMask = 1 << (int)DefaultLayers.Interactions;
        private readonly RaycastHit[] _hits = new RaycastHit[10];
        private readonly List<Interaction> _hitInteractions = new ();
        private int _hitsCount;
        private Interaction _activeInteraction;
        private int[] _outlineRenderers = Array.Empty<int>();
        private const string ICON = "d_EventTrigger Icon";
        private readonly GUIStyle _roundedBoxStyle = new ();
        
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
            SceneView.duringSceneGui += OnSceneGUI;
        }
        
        public override void OnWillBeDeactivated()
        {
            SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Scene Interaction Deactivated"), .1f);
            SceneView.duringSceneGui -= OnSceneGUI;
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

        private Texture2D CreateRoundedRectTexture(int width, int height, int radius, Color fillColor)
        {
            var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            var clear = new Color(0, 0, 0, 0);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var pixelColor = clear;
                    
                    var insideMainRect = 
                        (x >= radius && x < width - radius) || 
                        (y >= radius && y < height - radius);

                    var insideCorner =
                        (x < radius && y < radius && Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius)) <= radius) || // Bottom-left
                        (x >= width - radius && y < radius && Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, radius)) <= radius) || // Bottom-right
                        (x < radius && y >= height - radius && Vector2.Distance(new Vector2(x, y), new Vector2(radius, height - radius - 1)) <= radius) || // Top-left
                        (x >= width - radius && y >= height - radius && Vector2.Distance(new Vector2(x, y), new Vector2(width - radius - 1, height - radius - 1)) <= radius); // Top-right

                    if (insideMainRect || insideCorner)
                    {
                        pixelColor = fillColor;
                    }

                    texture.SetPixel(x, y, pixelColor);
                }
            }
            texture.Apply();
            return texture;
        }

        private void InitRoundedBoxStyle(Color backgroundColor)
        {
            var roundedTexture = CreateRoundedRectTexture(64, 64, 8, backgroundColor);
            _roundedBoxStyle.normal.background = roundedTexture;
            _roundedBoxStyle.border = new RectOffset(12, 12, 12, 12);
            _roundedBoxStyle.padding = new RectOffset(10, 10, 10, 10);
            _roundedBoxStyle.alignment = TextAnchor.MiddleCenter;
            _roundedBoxStyle.normal.textColor = Color.white;
        }

        private void OnSceneGUI(SceneView sceneView) 
        {
            Handles.BeginGUI();

            if (Event.current.type == EventType.Repaint)
            {
                if (_roundedBoxStyle.normal.background == null)
                {
                    Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, .5f);
                    InitRoundedBoxStyle(backgroundColor);
                }

                var notificationText = "Interaction Mode: Active";
                var content = new GUIContent(notificationText);
                var textSize = _roundedBoxStyle.CalcSize(content);
                var horizontalMargin = 12f;
                var verticalMargin = 12f;
                var totalSize = new Vector2(textSize.x + horizontalMargin, textSize.y + verticalMargin);
                var xPos = (sceneView.position.width - totalSize.x) * 0.5f;
                var yPos = 10f;
                var rect = new Rect(xPos, yPos, totalSize.x, totalSize.y);

                GUI.Box(rect, notificationText, _roundedBoxStyle);
            }
            Handles.EndGUI();
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
