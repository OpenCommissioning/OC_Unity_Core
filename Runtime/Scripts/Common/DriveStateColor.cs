using System;
using System.Collections.Generic;
using OC.Components;
using UnityEngine;

namespace OC
{
    public class DriveStateColor : MonoBehaviour
    {
        [Header("Parameter")] 
        [SerializeField]
        private Drive _drive;
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
            if (_drive == null) return;
            _drive.State.OnValueChanged += OnDriveStateChanged;
        }

        private void OnDriveStateChanged(Drive.DriveState state)
        {
            switch (state)
            {
                case Drive.DriveState.Idle:
                    SetProperties(_initProperty);
                    break;
                case Drive.DriveState.IsRunningNegative:
                    SetProperties(_backwardProperty);
                    break;
                case Drive.DriveState.IsRunningPositive:
                    SetProperties(_forwardProperty);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void OnDisable()
        {
            if (_drive == null) return;
            _drive.State.OnValueChanged -= OnDriveStateChanged;
        }
        
        private void GetPropertyBlock()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _initProperty = new Dictionary<Renderer, MaterialPropertyBlock[]>();
            _forwardProperty = new Dictionary<Renderer, MaterialPropertyBlock[]>();
            _backwardProperty = new Dictionary<Renderer, MaterialPropertyBlock[]>();
            
            foreach (var item in _renderers)
            {
                var materials = item.materials;
                var initBlocks = new MaterialPropertyBlock[materials.Length];
                var forwardBlocks = new MaterialPropertyBlock[materials.Length];
                var backwardBlocks = new MaterialPropertyBlock[materials.Length];

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

        private void SetProperties(Dictionary<Renderer, MaterialPropertyBlock[]> propertyBlocksMap)
        {
            foreach (var item in propertyBlocksMap)
            {
                for (var i = 0; i < item.Key.materials.Length; i++)
                {
                    item.Key.SetPropertyBlock(item.Value[i], i);
                }
            }
        }
    }
}
