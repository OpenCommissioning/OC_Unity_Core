using System.Collections;
using OC.Interactions;
using UnityEngine;

namespace OC
{
    [AddComponentMenu("Open Commissioning/Utils/Button Press Animation")]
    [DisallowMultipleComponent]
    public class ButtonPressAnimation : MonoBehaviour
    {
        public bool Pressed
        {
            get => _pressed;
            set => SetState(value);
        }

        [Header("State")] 
        [SerializeField]
        private bool _pressed;

        [Header("Settings")] 
        [SerializeField] 
        private Button _button;
        [SerializeField]
        private float _duration = 0.1f;
        [SerializeField] 
        private float _offset = 0.01f;
        [SerializeField]
        private Vector3 _direction = Vector3.forward;

        private bool _isValid;
        private Vector3 _startPosition;
        private Vector3 _endPosition;

        private void OnEnable()
        {
            Initialize();
            if (_button != null)
            {
                _button.Pressed.ValueChanged += SetState;
                SetState(_button.Pressed.Value);
            }
        }

        private void OnDisable()
        {
            if (_button != null) _button.Pressed.ValueChanged -= SetState;
        }

        private void OnValidate()
        {
            Initialize();
        }

        private void Initialize()
        {
            _startPosition = transform.localPosition;
            _endPosition = _startPosition + transform.localRotation * _direction * _offset;
        }

        private void StartAnimation(bool value)
        {
            StopAllCoroutines();
            StartCoroutine(Animation(value));
        }

        private void SetState(bool value)
        {
            if (_pressed == value) return;
            _pressed = value;
            StartAnimation(value);
        }

        private IEnumerator Animation(bool pressed)
        {
            if (_duration > 0.01f)
            {
                var startPosition = pressed ? _startPosition : _endPosition;
                var targetPosition = pressed ? _endPosition : _startPosition;

                for (var time = 0f; time < _duration; time += Time.deltaTime)
                {
                    var progress = time / _duration;
                    transform.localPosition = Vector3.Lerp(startPosition, targetPosition, progress);
                    yield return null;
                }
            }
            else
            {
                transform.localPosition = pressed ? _endPosition : _startPosition;
            }
        }
    }
}
