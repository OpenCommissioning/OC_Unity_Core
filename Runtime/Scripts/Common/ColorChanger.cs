using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OC
{
    [AddComponentMenu("Open Commissioning/Utils/Color Changer")]
    [DisallowMultipleComponent]
    public class ColorChanger : MonoBehaviour
    {
        public bool Enable
        {
            get => _enable;
            set
            {
                if (_enable == value) return;
                _enable = value;
                EnableEmission(value); 
            }
        }
        
        public Color Color
        {
            get => _color;
            set
            {
                if (_color == value) return;
                _color = value;
                UpdatePropertyBlocks();
            }
        }

        [Header("Control")] 
        [SerializeField]
        private bool _enable;
        
        [Header("Settings")]
        [SerializeField]
        private Color _color = Color.cyan;
        [SerializeField] 
        private int _emission = 10;

        private int _targetEmission;
        private const float STEP = 0.1f;
        private List<Renderer> _renderers = new ();
        private MaterialPropertyBlock _propertyBlock;

        private static readonly int PropertyColor = Shader.PropertyToID("_BaseColor");
        private static readonly int PropertyEmission = Shader.PropertyToID("_EmissionColor");
        
        private void Start()
        {
            Initilize();
            UpdatePropertyBlocks();
        }

        private void OnValidate()
        {
            EnableEmission(_enable);
        }

        private void EnableEmission(bool enable)
        {
            _targetEmission = enable ? _emission : 0;
            UpdatePropertyBlocks();
        }

        private void Initilize()
        {
            var material = Resources.Load<Material>("Materials/Utils/EmissionOn");
            _renderers = gameObject.GetComponentsInChildren<Renderer>().ToList();
            _propertyBlock = new MaterialPropertyBlock();
            
            foreach (var item in _renderers)
            {
                item.material = material;
            }
        }

        private void UpdatePropertyBlocks()
        {
            if (!Application.isPlaying) Initilize();
            
            foreach (var item in _renderers)
            {
                item.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor(PropertyColor, _color);
                _propertyBlock.SetColor(PropertyEmission, _color * _targetEmission * STEP);
                item.SetPropertyBlock(_propertyBlock);
            }
            
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}