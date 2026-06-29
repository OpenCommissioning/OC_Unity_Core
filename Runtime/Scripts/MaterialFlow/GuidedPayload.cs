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
        [SerializeField]
        private bool _preserveEntryLateralOffset;

        private Payload _payload;
        private Transform _transform;
        private Rigidbody _rigidbody;
        private ConfigurableJoint _joint;
        private float _angleOffset;
        private Vector3 _entryLateralOffsetLocal;
        private readonly RaycastHit[] _raycastHits = new RaycastHit[2];
        private readonly List<Transport> _transports = new List<Transport>();

        private void OnEnable()
        {
            _transform = GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody>();
            _payload = GetComponent<Payload>();
            _payload.ControlState.OnValueChanged += ControlStateChanged;
        }

        private void OnDisable()
        {
            _payload.ControlState.OnValueChanged -= ControlStateChanged;
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
            _transports.Remove(transport);
            if (transport != _transport) return;
            DecoupleFromTransport();
        }

        public void Decouple()
        {
            _transports.Clear();
            DecoupleFromTransport();
        }

        private void ControlStateChanged(ControlState state)
        {
            if (state != ControlState.Busy) return;
            DestroyJoint();
        }

        private void Raycast()
        {
            var raycastPosition = _transform.position + _transform.up * (_raycastLength * 0.5f);
            var hits = Physics.RaycastNonAlloc(raycastPosition, _transform.TransformDirection(Vector3.down),
                _raycastHits, _raycastLength, _raycastLayer);

            if (hits == 0)
            {
                DecoupleFromTransport();
                return;
            }

            var hitIndex = GetClosestHitIndex(hits);
            if (!_raycastHits[hitIndex].transform.TryGetComponent(out Transport transport))
            {
                DecoupleFromTransport();
                return;
            }

            if (!transport.IsGuiding)
            {
                DecoupleFromTransport();
                return;
            }

            if (transport == _transport) return;
            CoupleToTransport(transport);
        }

        private int GetClosestHitIndex(int hitCount)
        {
            var distance = Mathf.Infinity;
            var result = 0; 
            for (var i = 0; i < hitCount; i++)
            {
                if (distance < _raycastHits[i].distance) continue;
                distance = _raycastHits[i].distance;
                result = i;
            }

            return result;
        }

        private void CoupleToTransport(Transport transport)
        {
            _transport = transport;
            _angleOffset = GetOffsetAngle(_transport);
            CaptureEntryLateralOffset();
            CreateJoint();
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
            _joint.connectedAnchor = GetGuidedAnchorPoint();
            var angleRotation = Quaternion.AngleAxis(_angleOffset, Vector3.up);
            _rigidbody.transform.rotation = Quaternion.LookRotation(normal, Vector3.up) * angleRotation;
            _joint.axis = angleRotation * Vector3.forward;
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
            var angle  = Vector3.SignedAngle(normal, _transform.forward, Vector3.up);
            return Mathf.Round(angle / 90f) * 90f;
        }

        private void CaptureEntryLateralOffset()
        {
            if (!_preserveEntryLateralOffset)
            {
                _entryLateralOffsetLocal = Vector3.zero;
                return;
            }

            var closest = _transport.GetClosetPoint(_transform.position);
            var direction = _transport.GetDirection(_transform.position);
            var worldOffset = Vector3.ProjectOnPlane(_transform.position - closest, direction);
            var beltRotation = Quaternion.LookRotation(direction, Vector3.up);
            _entryLateralOffsetLocal = Quaternion.Inverse(beltRotation) * worldOffset;
        }

        private Vector3 GetGuidedAnchorPoint()
        {
            var closest = _transport.GetClosetPoint(_transform.position);
            if (!_preserveEntryLateralOffset) return closest;

            var direction = _transport.GetDirection(_transform.position);
            var beltRotation = Quaternion.LookRotation(direction, Vector3.up);
            return closest + beltRotation * _entryLateralOffsetLocal;
        }

        private void DecoupleFromTransport()
        {
            DestroyJoint();
            ClearTransportCoupling();
        }

        private void DestroyJoint()
        {
            if (_joint == null) return;

            if (Application.isPlaying) Destroy(_joint);
            else DestroyImmediate(_joint);
            _joint = null;
        }

        private void ClearTransportCoupling()
        {
            _transport = null;
            _entryLateralOffsetLocal = Vector3.zero;
        }

        private void OnDrawGizmos()
        {
            if (!_showGizmos) return;
            if (_transport == null || !_transport.IsGuiding) return;

            var point = GetGuidedAnchorPoint();
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
