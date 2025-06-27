using System.Collections.Generic;
using UnityEngine;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Actor/Transport Curved")]
    public class TransportCurved : Transport
    {
        [SerializeField]
        private Property<float> _angle = new (90);
        [SerializeField]
        private Property<float> _radius = new (1);
        [SerializeField] 
        private Property<float> _smoothness = new (0.1f);

        private Vector3 _direction;
        private Vector3 _translationSurfaceDelta;
        private Quaternion _rotationSurfaceDelta;
        private float _arcLength;
        private float _angularSpeedFactor;
        private float _angularDeltaSpeed;
        private int _sectionsCount;
        private float _angleStep;
        private Vector3 _rotationPoint;
        private Rectangle _sideProfile;
        private Rectangle[] _sections;

        protected new void OnEnable()
        {
            base.OnEnable();
            _angle.OnValueChanged += OnShapeChanged;
            _radius.OnValueChanged += OnShapeChanged;
            _smoothness.OnValueChanged += OnShapeChanged;
        }
        
        protected new void OnDisable()
        {
            base.OnDisable();
            _angle.OnValueChanged -= OnShapeChanged;
            _radius.OnValueChanged -= OnShapeChanged;
            _smoothness.OnValueChanged -= OnShapeChanged;
        }

        protected new void OnValidate()
        {
            base.OnValidate();
            _angle.Value = Mathf.Clamp(_angle.Value, -90, 90);
            _radius.Value = Mathf.Clamp(_radius.Value, _width * 0.5f, Mathf.Infinity);
            _smoothness.Value = Mathf.Clamp(_smoothness.Value, 0.001f, 1);
            _angle.OnValidate();
            _radius.OnValidate();
            _smoothness.OnValidate();
        }

        private void OnShapeChanged(float _)
        {
            CreateSurface();
        }

        protected override void CreateSurface()
        {
            if (Mathf.Abs(_angle) < Utils.TOLERANCE_HALF) return;

            _direction = Vector3.up * Mathf.Sign(_angle);
            _angularSpeedFactor = 1 / (Mathf.Deg2Rad * _radius);
            
            _arcLength = Mathf.PI * _radius * (Mathf.Abs(_angle) / 180);
            _sectionsCount = Mathf.CeilToInt(_arcLength / _smoothness) + 1;
            _angleStep = Mathf.Abs(_angle) / (_sectionsCount - 1);
            _rotationPoint = Vector3.right * (Mathf.Sign(_angle) * _radius);
            
            _pathPoints = new Vector3[_sectionsCount];
            for (var i = 0; i < _pathPoints.Length; i++)
            {                
                _pathPoints[i] = Math.RotatePointAroundPivot(Vector3.zero, _rotationPoint, (Vector3.up * (Mathf.Sign(_angle) * _angleStep * i)));
            }
            
            _sideProfile = new Rectangle();
            var halfWidth = Size.Value.x * 0.5f;
            _sideProfile.Points[0] = new Vector3(-halfWidth, 0, 0);
            _sideProfile.Points[1] = new Vector3(halfWidth, 0, 0);
            _sideProfile.Points[2] = new Vector3(halfWidth, -Size.Value.y, 0);
            _sideProfile.Points[3] = new Vector3(-halfWidth, -Size.Value.y, 0);
            
            _sections = new Rectangle[_sectionsCount];
            for (var i = 0; i < _sectionsCount; i++)
            {
                _sections[i] = new Rectangle();
                for (var j = 0; j < _sideProfile.Points.Length; j++)
                {
                    _sections[i].Points[j] = Math.RotatePointAroundPivot(_sideProfile.Points[j], _rotationPoint, Vector3.up * (Mathf.Sign(_angle) * _angleStep * i));
                }
            }

            var points = new List<Vector3>();
            points.AddRange(_sections[0].Points);
            points.AddRange(_sections[^1].Points);

            var bounds = GeometryUtility.CalculateBounds(points.ToArray(), Matrix4x4.identity);
            _collider.size = bounds.size;
            _collider.center = bounds.center;
        }

        protected override void MoveSurface(float speed)
        {
            _translationSurfaceDelta = _rigidbody.transform.forward * (speed * Time.fixedDeltaTime);
            _rigidbody.position -= _translationSurfaceDelta;
            _rigidbody.MovePosition(_rigidbody.position + _translationSurfaceDelta);

            _angularDeltaSpeed = _angularSpeedFactor * speed * Time.fixedDeltaTime;
            _rigidbody.rotation *= Quaternion.AngleAxis(-_angularDeltaSpeed, _direction);
            _rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.AngleAxis(_angularDeltaSpeed, _direction));
        }

        protected override void DrawSurface()
        {
            Gizmos.color = Color.blue;
            DrawSurfaceProfile(_sections[0]);
            for (var i = 1; i < _sections.Length; i++)
            {
                DrawSurfaceStep(_sections[i - 1], _sections[i]);
            }
            DrawSurfaceProfile(_sections[^1]);
        }

        public override Vector3 GetClosetPoint(Vector3 point)
        {
            var localPoint = transform.InverseTransformPoint(point);
            localPoint.y = 0;
            var initDirection = _pathPoints[0] - _rotationPoint;
            var pointDirection = localPoint - _rotationPoint;
            var angle = Vector3.SignedAngle(initDirection, pointDirection, Vector3.up * Mathf.Sign(_angle));            
            angle = Mathf.Clamp(angle, 0, Mathf.Abs(_angle));
            var result = Math.RotatePointAroundPivot(_pathPoints[0], _rotationPoint, Vector3.up * (angle * Mathf.Sign(_angle)));
            return transform.TransformPoint(result);
        }

        public override Vector3 GetDirection(Vector3 point)
        {
            var localPoint = transform.InverseTransformPoint(point);
            localPoint.y = 0;
            var initDirection = _pathPoints[0] - _rotationPoint;
            var pointDirection = localPoint - _rotationPoint;
            var angle = Vector3.SignedAngle(initDirection, pointDirection, Vector3.up * Mathf.Sign(_angle));
            angle = Mathf.Clamp(angle, 0, Mathf.Abs(_angle));

            var tangent = Quaternion.AngleAxis(angle, Vector3.up * Mathf.Sign(_angle)) * transform.forward;
            return tangent;
        }
        
        private class Rectangle
        {
            public readonly Vector3[] Points = new Vector3[4];
        }
        
        private void DrawSurfaceProfile(Rectangle rectangle)
        {
            Gizmos.DrawLine(rectangle.Points[0], rectangle.Points[1]);
            Gizmos.DrawLine(rectangle.Points[1], rectangle.Points[2]);
            Gizmos.DrawLine(rectangle.Points[2], rectangle.Points[3]);
            Gizmos.DrawLine(rectangle.Points[3], rectangle.Points[0]);
        }

        private void DrawSurfaceStep(Rectangle rectangle, Rectangle lastRechtangle)
        {
            for (var i = 0; i < rectangle.Points.Length; i++)
            {
                Gizmos.DrawLine(rectangle.Points[i], lastRechtangle.Points[i]);
            }
        }
    }
}