using System.Collections;
using OC.Interactions;
using UnityEngine;

namespace OC
{
    [AddComponentMenu("Open Commissioning/Utils/Switch Rotary Animation")]
    [DisallowMultipleComponent]
    public class SwitchRotationAnimation : MonoBehaviour
    {
        public float Angle
        {
            get => _angle;
            set
            {
                if (System.Math.Abs(_angle - value) < Utils.TOLERANCE) return;
                StartAnimation(value);
            }
        }

        [Header("State")] 
        [SerializeField]
        private float _angle;
        
        [Header("Settings")]
        [SerializeField] 
        private SwitchRotary _switch;
        [SerializeField]
        private float _duration = 0.1f;
        [SerializeField]
        private Vector3 _direction = Vector3.forward;

        private bool _isValid;
        private float _lastAngle;
        private Quaternion _initRotation;

        private void OnEnable()
        {
            _initRotation = transform.localRotation;
            if (_switch != null)
            {
                _switch.Angle.ValueChanged += StartAnimation;
                SetAngle(_switch.Angle.Value);
            }
        }
        
        private void OnDisable()
        {
            if (_switch != null) _switch.Angle.ValueChanged -= StartAnimation;
        }

        private void StartAnimation(float value)
        {
            StopAllCoroutines();
            StartCoroutine(Animation(value));
        }

        private IEnumerator Animation(float angle)
        {
            if (_duration > 0)
            {
                var start = _angle;

                for (var time = 0f; time < _duration; time += Time.deltaTime)
                {
                    var progress = time / _duration;
                    _angle = Mathf.Lerp(start, angle, progress);
                    SetAngle(_angle);
                    yield return null;
                }
            }
            
            SetAngle(angle);
        }

        private void SetAngle(float angle)
        {
            transform.localRotation = _initRotation * Quaternion.AngleAxis(angle, _direction);
        }
    }
}
