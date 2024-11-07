using System.Collections;
using UnityEngine;

namespace OC.MaterialFlow
{
    [SelectionBase]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class PlatformSurface : MonoBehaviour
    {
        [SerializeField] 
        private float _delay = 0.5f;
        [SerializeField]
        private float _force = 10f;
        private BoxCollider _boxCollider;
        private Rigidbody _rigidbody;
        
        private void Start()
        {
            GetReferences();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.TryGetComponent(out Payload payload)) return;
            if (payload.TryGetComponent(out GuidedPayload guidedPayload)) guidedPayload.Decouple();
            payload.transform.parent = transform;
            StopAllCoroutines();
            StartCoroutine(CreateFixedJointCorutine(payload));
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.TryGetComponent(out Payload payload))
            {
                payload.transform.parent = Pool.Instance.transform;
            }
        }

        private IEnumerator CreateFixedJointCorutine(Payload payload)
        {
            yield return new WaitForSeconds(_delay);
            if (!TryGetComponent(out FixedJoint joint)) joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = payload.GetComponent<Rigidbody>();
            joint.breakForce = _force;
            joint.breakTorque = _force;
        }
        
        private void GetReferences()
        {
             if (_boxCollider == null)
             {
                 _boxCollider = GetComponent<BoxCollider>();
                 _boxCollider.isTrigger = false;
             }

             if (_rigidbody != null) return;
             _rigidbody = GetComponent<Rigidbody>();
             _rigidbody.isKinematic = true;
             _rigidbody.useGravity = false;
             _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
    }
}