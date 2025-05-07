using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace OC.Interactions
{
    [AddComponentMenu("Open Commissioning/Interactions/Door")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class Door : MonoBehaviour, IInteractable
    {
        public IProperty<bool> Lock => _lock;
        public IPropertyReadOnly<bool> Closed => _closed;
        public IPropertyReadOnly<bool> Locked => _locked;

        [SerializeField]
        private Property<bool> _lock = new (false);
        [SerializeField]
        private Property<bool> _closed = new (true);
        [SerializeField]
        private Property<bool> _locked = new (false);

        [SerializeField] 
        private Transform _target;
        [SerializeField] 
        private float _duration = 0.5f;

        public UnityEvent OnOpenEvent;
        public UnityEvent OnCloseEvent;

        private bool _isValid;
        private Vector3 _initPosition;
        private Quaternion _initRotation;
        private Vector3 _targetLocalPosition;
        private Quaternion _targetLocalRotation;

        private void Start()
        {
            _initPosition = transform.localPosition;
            _initRotation = transform.localRotation;

            if (_target == null)
            {
                Logging.Logger.Log(LogType.Error, "Door target isn't assigned!", this);
                return;
            }
            
            _targetLocalRotation = _initRotation * _target.localRotation;
            _targetLocalPosition = _initPosition + _targetLocalRotation * _target.localPosition;
            _isValid = true;
        }

        private void LateUpdate()
        {
            _locked.Value = _lock.Value && _closed.Value;
        }

        public void Open()
        {
            if (!_closed.Value || _lock.Value) return;
            if (!_isValid) return;
            StopAllCoroutines();
            StartCoroutine(OpenAnimation());
        }

        public void Close()
        {
            if (_closed.Value) return;
            if (!_isValid) return;
            StopAllCoroutines();
            StartCoroutine(CloseAnimation());
        }

        public void SetLock(bool value)
        {
            _lock.Value = value;
        }

        private IEnumerator OpenAnimation()
        {
            OnOpenEvent?.Invoke();
            _closed.Value = false;
            yield return transform.Interpolate(_targetLocalPosition, _targetLocalRotation, _duration, MotionUtils.TransformSpace.Local);
        }

        private IEnumerator CloseAnimation()
        {
            OnCloseEvent?.Invoke();
            yield return transform.Interpolate(_initPosition, _initRotation, _duration, MotionUtils.TransformSpace.Local);
            _closed.Value = true;
        }
        
        public void Click()
        {
            if (!Application.isPlaying) return;
            
            if (!_isValid)
            {
                Logging.Logger.Log(LogType.Error, "Door target isn't assigned!", this);
                return;
            }
            
            if (_closed.Value)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
    }
}
