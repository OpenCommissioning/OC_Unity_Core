using System.Collections;
using OC.Components;
using UnityEngine;

namespace OC.MaterialFlow
{
    [RequireComponent(typeof(BoxCollider))]
    [DisallowMultipleComponent]
    public class SnapToTransport : MonoBehaviour
    {
        [SerializeField]
        private Transport _transport;
        [SerializeField] 
        [Min(0.001f)]
        private float _duration = 1f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Payload payload))
            {
                StartCoroutine(Snap(payload, _duration));
            }
        }

        private IEnumerator Snap(Payload payload, float duration)
        {
            var normal = _transport.GetDirection(payload.transform.position);
            var angleOffset = GetOffsetAngle(_transport, payload);
            var startRotation = payload.transform.rotation;
            var targetRotation = Quaternion.LookRotation(normal, Vector3.up) * Quaternion.AngleAxis(angleOffset, Vector3.up);
            var offsetAlongX = _transport.transform.InverseTransformPoint(payload.transform.position).x;
            var compensationSpeed = offsetAlongX / _duration;

            for (var time = 0f; time <= duration; time += Time.deltaTime)
            {
                var t = Mathf.Clamp01(time / duration);
                payload.transform.rotation = Quaternion.Lerp(startRotation,targetRotation,t);
                payload.transform.Translate(_transport.transform.right * (-compensationSpeed * Time.deltaTime), Space.World);
                yield return null;
            }
        }
        
        private float GetOffsetAngle(Transport transport, Payload payload)
        {
            var normal = transport.GetDirection(payload.transform.position);
            var angle  = Vector3.SignedAngle(normal, payload.transform.forward, Vector3.up);
            return Mathf.Round(angle / 90f) * 90f;
        }
    }
}
