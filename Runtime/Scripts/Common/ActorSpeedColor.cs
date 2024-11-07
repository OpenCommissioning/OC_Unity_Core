using System.Collections.Generic;
using OC.Components;
using UnityEngine;

namespace OC
{
    public class ActorSpeedColor : MonoBehaviour
    {
        [Header("Parameter")] 
        [SerializeField]
        private Actor _actor;
        [SerializeField] 
        private Color _colorForward = Color.green;
        [SerializeField] 
        private Color _colorBackward = Color.blue;
        [SerializeField]
        private bool _invertDirection;
        
        private Renderer[] _renderers;
        private Dictionary<Renderer, MaterialPropertyBlock[]> _initProperty;
        private Dictionary<Renderer, MaterialPropertyBlock[]> _forwardProperty;
        private Dictionary<Renderer, MaterialPropertyBlock[]> _backwardProperty;
        
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private void OnEnable()
        {
            GetPropertyBlock();
            if (_actor == null) return;
            if (_actor is DriveSpeed) _actor.Value.ValueChanged += OnSpeedChanged;
            if (_actor is DrivePosition drivePosition) drivePosition.Delta.ValueChanged += OnSpeedChanged;
        }

        private void OnDisable()
        {
            if (_actor == null) return;
            if (_actor is DriveSpeed) _actor.Value.ValueChanged -= OnSpeedChanged;
            if (_actor is DrivePosition drivePosition) drivePosition.Delta.ValueChanged -= OnSpeedChanged;
        }
        
        private void OnSpeedChanged(float value)
        {
            if (Math.FastApproximately(value, 0))
            {
                SetColor(false,false);
            }
            else switch (value)
            {
                case > 0:
                    SetColor(true, !_invertDirection);
                    break;
                case < 0:
                    SetColor(true, _invertDirection);
                    break;
            }
        }
        
        private void GetPropertyBlock()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _initProperty = new Dictionary<Renderer, MaterialPropertyBlock[]>();
            _forwardProperty = new Dictionary<Renderer, MaterialPropertyBlock[]>();
            _backwardProperty = new Dictionary<Renderer, MaterialPropertyBlock[]>();
            
            foreach (var item in _renderers)
            {
                var initBlocks = new MaterialPropertyBlock[item.materials.Length];
                var forwardBlocks = new MaterialPropertyBlock[item.materials.Length];
                var backwardBlocks = new MaterialPropertyBlock[item.materials.Length];

                for (var i = 0; i < item.materials.Length; i++)
                {
                    var init = new MaterialPropertyBlock();
                    item.GetPropertyBlock(init, i);
                    initBlocks[i] = init;

                    var forwardBlock = new MaterialPropertyBlock();
                    forwardBlock.SetColor(BaseColor, _colorForward);
                    forwardBlocks[i] = forwardBlock;
                    
                    var backwardBlock = new MaterialPropertyBlock();
                    backwardBlock.SetColor(BaseColor, _colorBackward);
                    backwardBlocks[i] = backwardBlock;
                }

                _initProperty.Add(item, initBlocks);
                _forwardProperty.Add(item, forwardBlocks);
                _backwardProperty.Add(item, backwardBlocks);
            }
        }

        private void SetColor(bool enable, bool forwardDirection)
        {
            if (enable)
            {
                if (forwardDirection)
                {
                    foreach (var item in _forwardProperty)
                    {
                        for (var i = 0; i < item.Key.materials.Length; i++)
                        {
                            item.Key.SetPropertyBlock(item.Value[i], i);
                        }
                    }
                }
                else
                {
                    foreach (var item in _backwardProperty)
                    {
                        for (var i = 0; i < item.Key.materials.Length; i++)
                        {
                            item.Key.SetPropertyBlock(item.Value[i], i);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in _initProperty)
                {
                    for (var i = 0; i < item.Key.materials.Length; i++)
                    {
                        item.Key.SetPropertyBlock(item.Value[i], i);
                    }
                }
            }
        }
    }
}
