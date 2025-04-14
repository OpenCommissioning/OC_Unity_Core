using System.Collections.Generic;
using OC.Components;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OC.MaterialFlow
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Payload))]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class GuidedPayload: MonoBehaviour
    {
        [Header("Collision")]
        [SerializeField]
        private Transport _transport;

        [Header("Settings")] 
        [SerializeField] 
        private float _raycastLength = 0.06f;
        [SerializeField] 
        private LayerMask _raycastLayer = 1 << (int)DefaultLayers.Transport;
        [SerializeField]
        private bool _showGizmos;

        private Payload _payload;
        private Transform _transform;
        private Transport _lastTransport;
        private Rigidbody _rigidbody;
        private ConfigurableJoint _joint;
        private float _angleOffset;
        private ControlState _lastState;
        private readonly RaycastHit[] _raycastHits = new RaycastHit[2];
        private readonly List<Transport> _transports = new List<Transport>();

        private void OnEnable()
        {
            _transform = GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody>();
            _payload = GetComponent<Payload>();
            _payload.ControlState.ValueChanged += ControlStateChaged;
        }

        private void OnDisable()
        {
            _payload.ControlState.ValueChanged -= ControlStateChaged;
        }

        private void FixedUpdate()
        {
            if (_payload.ControlState.Value != ControlState.Ready) return;
            if (_transports.Count <= 0) return;
            Raycast();
            Move();
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.gameObject.TryGetComponent(out Transport transport)) return;
            if (!transport.IsGuiding) return;
            if (!_transports.Contains(transport)) _transports.Add(transport);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!collision.gameObject.TryGetComponent(out Transport transport)) return;
            if (!transport.IsGuiding) return;
            if (_transports.Contains(transport)) _transports.Remove(transport);
            if (transport != _transport) return;
            _transport = null;
            if (_joint != null) Destroy(_joint);
        }

        public void Decouple()
        {
            if (_joint != null) Destroy(_joint);
            _transports.Clear();
            _transport = null;
        }

        private void ControlStateChaged(ControlState state)
        {
            if (state != ControlState.Busy) return;
            if (_joint != null) Destroy(_joint);
        }

        private void Raycast()
        {
            var raycastPosition = transform.position + transform.up * (_raycastLength * 0.5f);
            var hits = Physics.RaycastNonAlloc(raycastPosition, transform.TransformDirection(Vector3.down),
                _raycastHits, _raycastLength, _raycastLayer);

            if (hits == 0)
            {
                if (_joint != null) DestroyImmediate(_joint);
                return;
            }

            var hitIndex = 0;
            if (hits > 1)
            {
                hitIndex = GetClosestHitIndex(_raycastHits);
            }
            
            if (!_raycastHits[hitIndex].transform.TryGetComponent(out Transport transport)) return;
            if (transport == _transport) return;
            _transport = transport;

            if (_transport.IsGuiding)
            {
                _angleOffset = GetOffsetAngle(_transport);
                CreateJoint();
            }
            else
            {
                if (_joint != null) DestroyImmediate(_joint);
            }
        }

        private int GetClosestHitIndex(IReadOnlyList<RaycastHit> hits)
        {
            var distance = Mathf.Infinity;
            var result = 0; 
            for (var i = 0; i < hits.Count; i++)
            {
                if (distance < hits[i].distance) continue;
                distance = hits[i].distance;
                result = i;
            }

            return result;
        }
        
        private void CreateJoint()
        {
            _joint = TryGetComponent(out ConfigurableJoint joint) ? joint : gameObject.AddComponent<ConfigurableJoint>();
            _joint.anchor = new Vector3(0, 0, 0);
            _joint.autoConfigureConnectedAnchor = false;
            _joint.xMotion = ConfigurableJointMotion.Free;
            _joint.yMotion = ConfigurableJointMotion.Locked;
            _joint.zMotion = ConfigurableJointMotion.Locked;
            _joint.angularXMotion = ConfigurableJointMotion.Locked;
#if UNITY_6000_0_OR_NEWER
            _joint.angularYMotion = ConfigurableJointMotion.Free;
#else
            _joint.angularYMotion = ConfigurableJointMotion.Locked;
#endif
            _joint.angularZMotion = ConfigurableJointMotion.Locked;
        }

        private void Move()
        {
            if (_joint == null) return;
            if (_transport == null) return;
            
            var normal = _transport.GetDirection(_transform.position);
            _joint.connectedAnchor =  _transport.GetClosetPoint(_transform.position);
            _rigidbody.transform.rotation = Quaternion.LookRotation(normal, Vector3.up) * Quaternion.AngleAxis(_angleOffset, Vector3.up);
            _joint.axis = Quaternion.AngleAxis(_angleOffset, Vector3.up) * Vector3.forward;
#if UNITY_6000_0_OR_NEWER
            if (!_rigidbody.isKinematic) _rigidbody.linearVelocity = normal * _transport.Value.Value;
            _rigidbody.angularVelocity = Vector3.zero;
#else
            if (!_rigidbody.isKinematic) _rigidbody.velocity = normal * _transport.Value.Value;
#endif
        }

        private float GetOffsetAngle(Transport transport)
        {
            var normal = transport.GetDirection(_transform.position);
            var angle  = Vector3.SignedAngle(normal, transform.forward, Vector3.up);
            return Mathf.Round(angle / 90f) * 90f;
        }

        private void OnDrawGizmos()
        {
            if (!_showGizmos) return;
            if (_transport == null) return;

            var point = _transport.GetClosetPoint(_transform.position);
            var normal = _transport.GetDirection(_transform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(point, 0.02f);
            Gizmos.DrawLine(transform.position, transform.position + normal);
#if UNITY_EDITOR
            Handles.PositionHandle(point, Quaternion.LookRotation(normal, Vector3.up));     
#endif
        }
    }
}
