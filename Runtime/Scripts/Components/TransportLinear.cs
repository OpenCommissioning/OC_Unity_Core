using UnityEngine;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Actor/Transport Linear")]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class TransportLinear : Transport
    {
        private Vector3 _moveSurfaceDelta;

        protected override void CreateSurface()
        {
            _pathPoints = new Vector3[2];
            _pathPoints[0] = Vector3.zero;
            _pathPoints[1] = Vector3.forward * Size.Value.z;
            _collider.size = Size.Value;
            _collider.center = new Vector3(0, -Size.Value.y * 0.5f, Size.Value.z * 0.5f);
        }

        protected override void MoveSurface(float speed)
        {
            _moveSurfaceDelta = _rigidbody.transform.forward * (speed * Time.fixedDeltaTime);
            _rigidbody.position -= _moveSurfaceDelta;
            _rigidbody.MovePosition(_rigidbody.position + _moveSurfaceDelta);
        }

        protected override void DrawSurface()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(_collider.center, _collider.size);
        }

        public override Vector3 GetClosetPoint(Vector3 point)
        {
            var localPoint = transform.InverseTransformPoint(point);
            var newPoint = Math.GetClosestPointOnFiniteLine(localPoint, _pathPoints[0], _pathPoints[1]);
            return transform.TransformPoint(newPoint);
        }

        public override Vector3 GetDirection(Vector3 point)
        {
            return transform.forward.normalized;
        }
    }
}