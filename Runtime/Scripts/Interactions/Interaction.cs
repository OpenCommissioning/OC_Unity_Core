using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OC.Interactions
{
    [AddComponentMenu("Open Commissioning/Interactions/Interaction")]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider))]
    public class Interaction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public GameObject Target
        {
            get
            {
                if (_target == null) _target = gameObject;
                return _target;
            }
            set => _target = value;
        }

        public InteractionMode Mode
        {
            get => _mode;
            set => _mode = value;
        }

        public IProperty<InteractionState> State => _state;

        public List<Renderer> Renderers => _renderers;

        [Header("State")]
        [SerializeField] 
        protected Property<InteractionState> _state = new (InteractionState.Idle);
        
        [Header("Settings")] 
        [SerializeField] 
        private InteractionMode _mode = InteractionMode.Hover | InteractionMode.Click;
        [SerializeField]
        protected GameObject _target;
        [SerializeField]
        protected bool _debug;

        public event Action OnDestroyAction;
        public UnityEvent OnPointerClickEvent;
        public UnityEvent OnPointerDownEvent;
        public UnityEvent OnPointerUpEvent;

        private List<Renderer> _renderers;
        
        protected void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>().ToList();
        }

        protected void OnDestroy()
        {
            OnDestroyAction?.Invoke();
        }

        private void Reset()
        {
            BoundBoxColliderSize();
            gameObject.layer = (int)DefaultLayers.Interactions;
        }

        [Flags]
        public enum InteractionMode
        {
            Selection = 1,
            Hover = 2,
            Click = 4,
            All = ~0
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isActiveAndEnabled) return;
            if (_debug) Debug.Log("Event: OnPointerEnter", this);
            if (_mode.HasFlag(InteractionMode.Hover)) _state.Value = _state.Value.SetFlag(InteractionState.Hovered);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isActiveAndEnabled) return;
            if (_debug) Debug.Log("Event: OnPointerExit", this);
            if (_mode.HasFlag(InteractionMode.Hover)) _state.Value = _state.Value.RemoveFlag(InteractionState.Hovered);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!isActiveAndEnabled) return;
            if (_debug) Debug.Log("Event: OnSelect", this);
            if (_mode.HasFlag(InteractionMode.Selection)) _state.Value = _state.Value.SetFlag(InteractionState.Selected);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (!isActiveAndEnabled) return;
            if (_debug) Debug.Log("Event: OnDeselect", this);
            if (_mode.HasFlag(InteractionMode.Selection)) _state.Value = _state.Value.RemoveFlag(InteractionState.Selected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isActiveAndEnabled) return;
            if (_debug) Debug.Log("Event: OnPointerClick", this);
            if (_mode.HasFlag(InteractionMode.Click)) OnPointerClickEvent?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isActiveAndEnabled) return;
            if (_debug) Debug.Log("Event: OnPointerDown", this);
            if (_mode.HasFlag(InteractionMode.Click)) OnPointerDownEvent?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isActiveAndEnabled) return;
            if (_debug) Debug.Log("Event: OnPointerUp", this);
            if (_mode.HasFlag(InteractionMode.Click)) OnPointerUpEvent?.Invoke();
        }

        [ContextMenu("Bound Box Collider Size", false, 100)]
        public void BoundBoxColliderSize()
        {
            if (Utils.TryBoundBoxColliderSize(gameObject, out var boxCollider))
            {
                boxCollider.isTrigger = true;
            }
        }
    }
}
