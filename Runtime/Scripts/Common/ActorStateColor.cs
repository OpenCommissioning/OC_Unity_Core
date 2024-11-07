using System;
using System.Collections.Generic;
using UnityEngine;

namespace OC
{
    public class ActorStateColor : MonoBehaviour
    {
        public ActorState State => _state;

        public bool Enable
        {
            get => _enable;
            set
            {
                _enable = value;
                UpdateState();
            }
        }

        public bool Warning
        {
            get => _warning;
            set
            {
                _warning = value;
                UpdateState();
            }
        }
        
        public bool Error
        {
            get => _error;
            set
            {
                _error = value;
                UpdateState();
            }
        }

        [Header("Control")] 
        [SerializeField]
        private Property<ActorState> _state = new (ActorState.Init);
        [SerializeField] 
        private bool _enable; 
        [SerializeField] 
        private bool _warning;
        [SerializeField] 
        private bool _error;

        [Header("Settings")]
        [SerializeField]
        private Color _colorEnable = Color.green;
        [SerializeField] 
        private Color _colorError = Color.red;
        [SerializeField] 
        private Color _colorWarning = Color.yellow;

        private Renderer[] _renderers;
        private Dictionary<Renderer, MaterialPropertyBlock[]> _initProperty;
        private Dictionary<Renderer, MaterialPropertyBlock[]> _enabledProperty;
        private Dictionary<Renderer, MaterialPropertyBlock[]> _warningProperty;
        private Dictionary<Renderer, MaterialPropertyBlock[]> _errorProperty;
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private void OnEnable()
        {
            GetPropertyBlock();
            _state.ValueChanged += OnStateChanged;
        }

        private void OnDisable()
        {
            _state.ValueChanged -= OnStateChanged;
        }

        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            _state.OnValidate();
        }

        private void UpdateState()
        {
            if (_error)
            {
                _state.Value = ActorState.Error;
                return;
            }

            if (_warning)
            {
                _state.Value = ActorState.Warning;
                return;
            }
            
            _state.Value = _enable ? ActorState.Enabled : ActorState.Init;
        }
        
        private void OnStateChanged(ActorState state)
        {
            switch (state)
            {
                case ActorState.Init:
                    SetPropertyBlock(_initProperty);
                    break;
                case ActorState.Enabled:
                    SetPropertyBlock(_enabledProperty);
                    break;
                case ActorState.Warning:
                    SetPropertyBlock(_warningProperty);
                    break;
                case ActorState.Error:
                    SetPropertyBlock(_errorProperty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void GetPropertyBlock()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _initProperty = new Dictionary<Renderer, MaterialPropertyBlock[]>();
            _enabledProperty = new Dictionary<Renderer, MaterialPropertyBlock[]>();
            _warningProperty = new Dictionary<Renderer, MaterialPropertyBlock[]>();
            _errorProperty = new Dictionary<Renderer, MaterialPropertyBlock[]>();
            
            foreach (var item in _renderers)
            {
                _initProperty.Add(item, GetPropertyBlocks(item));
                _enabledProperty.Add(item, CreatePropertyBlocks(item, _colorEnable));
                _warningProperty.Add(item, CreatePropertyBlocks(item, _colorWarning));
                _errorProperty.Add(item, CreatePropertyBlocks(item, _colorError));
            }
        }

        private static void SetPropertyBlock(Dictionary<Renderer, MaterialPropertyBlock[]> dictionary)
        {
            foreach (var item in dictionary)
            {
                for (var i = 0; i < item.Key.materials.Length; i++)
                {
                    item.Key.SetPropertyBlock(item.Value[i], i);
                }
            }
        }

        private static MaterialPropertyBlock[] GetPropertyBlocks(Renderer renderer)
        {
            var propertyBlocks = new MaterialPropertyBlock[renderer.materials.Length];
            for (var i = 0; i < renderer.materials.Length; i++)
            {
                var propertyBlock = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(propertyBlock, i);
                propertyBlocks[i] = propertyBlock;
            }
            return propertyBlocks;
        }

        private static MaterialPropertyBlock[] CreatePropertyBlocks(Renderer renderer, Color color)
        {
            var propertyBlocks = new MaterialPropertyBlock[renderer.materials.Length];
            for (var i = 0; i < renderer.materials.Length; i++)
            {
                var propertyBlock = new MaterialPropertyBlock();
                propertyBlock.SetColor(BaseColor, color);
                propertyBlocks[i] = propertyBlock;
            }
            return propertyBlocks;
        }
    }

    public enum ActorState
    {
        Init,
        Enabled,
        Warning,
        Error
    }
}
