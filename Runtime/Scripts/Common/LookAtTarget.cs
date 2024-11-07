using UnityEngine;

namespace OC
{
    [ExecuteInEditMode]
    public class LookAtTarget : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        private void LateUpdate()
        {
            if (_target == null) return;
            transform.LookAt(_target, _target.up);
        }
    }
}
