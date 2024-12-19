using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OC.Interactions
{
    [AddComponentMenu("Open Commissioning/Interaction")]
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

        [Header("State")]
        [SerializeField] 
        protected Property<InteractionState> _state = new (InteractionState.Enabled);
        
        [Header("Settings")] 
        [SerializeField] 
        private InteractionMode _mode = InteractionMode.Hover | InteractionMode.Click;
        [SerializeField]
        protected GameObject _target;

        public event Action OnDestroyAction;
        public UnityEvent OnPointerClickEvent;
        public UnityEvent OnPointerDownEvent;
        public UnityEvent OnPointerUpEvent;

        private Collider _collider;
        private List<Renderer> _renderers;
        private const uint LAYERMASK_HOVER = 2;
        private const uint LAYERMASK_SELECTION = 4;

        protected void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>().ToList();
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _collider.enabled = !_state.Value.HasFlag(InteractionState.Disabled);
            gameObject.layer = (int)DefaultLayers.Interactions;
        }

        private void OnEnable()
        {
            _state.ValueChanged += OnStateChanged;
        }
        
        private void OnDisable()
        {
            _state.ValueChanged -= OnStateChanged;
        }

        protected void OnDestroy()
        {
            OnDestroyAction?.Invoke();
        }
        
        private void OnStateChanged(InteractionState state)
        {
            switch (state)
            {
                case InteractionState.Disabled:
                    _collider.enabled = false;
                    _state.SetWithoutNotify(InteractionState.Disabled);
                    break;
                case InteractionState.Enabled:
                    _collider.enabled = true;
                    break;
                case InteractionState.Selected:
                    _collider.enabled = true;
                    break;
                case InteractionState.Hovered:
                    _collider.enabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            RefreshLayerMask();
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
            if (_state.Value.HasFlag(InteractionState.Disabled)) return;
            if (_mode.HasFlag(InteractionMode.Hover)) _state.Value = _state.Value.SetFlag(InteractionState.Hovered);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_state.Value.HasFlag(InteractionState.Disabled)) return;
            if (_mode.HasFlag(InteractionMode.Hover)) _state.Value = _state.Value.RemoveFlag(InteractionState.Hovered);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (_state.Value.HasFlag(InteractionState.Disabled)) return;
            if (_mode.HasFlag(InteractionMode.Selection)) _state.Value = _state.Value.SetFlag(InteractionState.Selected);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (_state.Value.HasFlag(InteractionState.Disabled)) return;
            if (_mode.HasFlag(InteractionMode.Selection)) _state.Value = _state.Value.RemoveFlag(InteractionState.Selected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_state.Value.HasFlag(InteractionState.Disabled)) return;
            if (_mode.HasFlag(InteractionMode.Click)) OnPointerClickEvent?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_state.Value.HasFlag(InteractionState.Disabled)) return;
            if (_mode.HasFlag(InteractionMode.Click)) OnPointerDownEvent?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_state.Value.HasFlag(InteractionState.Disabled)) return;
            if (_mode.HasFlag(InteractionMode.Click)) OnPointerUpEvent?.Invoke();
        }
        
        private void RefreshLayerMask()
        {
            if (!isActiveAndEnabled) return;

            if (!_state.Value.HasFlag(InteractionState.Disabled))
            {
                if (_state.Value.HasFlag(InteractionState.Selected))
                {
                    SetRenderLayerMask(LAYERMASK_SELECTION);
                    return;
                }

                if (_state.Value.HasFlag(InteractionState.Hovered))
                {
                    SetRenderLayerMask(LAYERMASK_HOVER);
                    return;
                }
            }

            SetRenderLayerMask(1);
        }
        
        private void SetRenderLayerMask(uint layerMask)
        {
            foreach (var item in _renderers)
            {
                item.renderingLayerMask = layerMask;
            }
        }
    }
}
