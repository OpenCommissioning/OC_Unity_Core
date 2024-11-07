using UnityEngine;

namespace OC
{
    public class PartTweening : MonoBehaviour
    {
        [SerializeField] private Vector3 _direction = Vector3.forward;
        [SerializeField] private float _speed = 1;
        [SerializeField] private float _amplitude = 1;
        [SerializeField] private bool _rotation;

        private Vector3 _initLocalPosition;
        private Quaternion _initLocatRotation;

        private void Start()
        {
            _initLocalPosition = transform.localPosition;
            _initLocatRotation = transform.localRotation;
        }

        private void Update()
        {
            if (!_rotation)
            {
                transform.localPosition = _initLocalPosition + transform.localRotation * _direction * (_amplitude * Mathf.Cos(_speed * Time.time));
            }
            else
            {
                transform.localRotation = _initLocatRotation * Quaternion.Euler(_direction * (_amplitude * Mathf.Cos(_speed * Time.time)));
            }
        }
    }
}
