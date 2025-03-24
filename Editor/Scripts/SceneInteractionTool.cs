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
        private GUIStyle _roundedBoxStyle = new GUIStyle();
        
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
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Color clear = new Color(0, 0, 0, 0);
            Color[] pixels = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool inside = true;

                    if (x < radius && y < radius)
                    {
                        if (Vector2.Distance(new Vector2(x, y), new Vector2(radius, radius)) > radius)
                            inside = false;
                    }
                    else if (x >= width - radius && y < radius)
                    {
                        if (Vector2.Distance(new Vector2(x, y), new Vector2(width - radius, radius)) > radius)
                            inside = false;
                    }
                    else if (x < radius && y >= height - radius)
                    {
                        if (Vector2.Distance(new Vector2(x, y), new Vector2(radius, height - radius)) > radius)
                            inside = false;
                    }
                    else if (x >= width - radius && y >= height - radius)
                    {
                        if (Vector2.Distance(new Vector2(x, y), new Vector2(width - radius, height - radius)) > radius)
                            inside = false;
                    }
                    
                    pixels[y * width + x] = inside ? fillColor : clear;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private void InitRoundedBoxStyle(Color backgroundColor)
        {
            Texture2D roundedTexture = CreateRoundedRectTexture(64, 64, 8, backgroundColor);
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

                string notificationText = "Interaction Mode: Active";
                GUIContent content = new GUIContent(notificationText);
                Vector2 textSize = _roundedBoxStyle.CalcSize(content);
                float horizontalMargin = 12f;
                float verticalMargin = 12f;
                Vector2 totalSize = new Vector2(textSize.x + horizontalMargin, textSize.y + verticalMargin);
                float xPos = (sceneView.position.width - totalSize.x) * 0.5f;
                float yPos = 10f;
                Rect rect = new Rect(xPos, yPos, totalSize.x, totalSize.y);

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
