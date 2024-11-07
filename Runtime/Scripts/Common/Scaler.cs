using System.Collections.Generic;
using UnityEngine;

namespace OC
{    
    public class Scaler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private float _amplitude = 0.1f;
        [SerializeField]
        private Vector3 _direction = Vector3.forward;
        [SerializeField]
        private bool _colorize;
        [SerializeField]
        private Color _colorEnd = Color.red;

        [Header("Control")]
        [SerializeField]
        private float _scaleT;

        private readonly List<Material> _materials = new List<Material>();
        private readonly List<Color> _initColors = new List<Color>();

        private void Start()
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                var materials = item.materials;
                foreach (var material in materials)
                {
                    _materials.Add(material);
                    _initColors.Add(material.color);
                }
            }
        }

        private void OnValidate()
        {
            SetStretch(_scaleT);
        }

        public void SetStretch(float T)
        {
            transform.localScale = Vector3.one + _direction * _amplitude * T; 
            
            if (_colorize)
            {
                for (var i = 0; i < _materials.Count; i++)
                {
                    _materials[i].color = Color.Lerp(_initColors[i], _colorEnd, Mathf.Abs(T));
                }
            }
        }
    }
}
