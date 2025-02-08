using OC.MaterialFlow;
using UnityEngine;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Actor/Transport")]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [SelectionBase]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public abstract class Transport : Actor, ICustomInspector
    {
        public Actor Actor
        {
            get => _actor;
            set => _actor = value;
        }

        public IProperty<Vector3> Size => _size;
        
        public bool IsGuiding => _isGuiding;
        
        [SerializeField]
        private Actor _actor;

        [SerializeField]
        protected float _width = 0.3f;
        [SerializeField]
        protected float _height = 0.1f;
        [SerializeField]
        protected float _length = 1;
        [SerializeField]
        protected float _factor = 0.001f;
        [SerializeField]
        private bool _isDynamic; 
        [SerializeField]
        private bool _isGuiding;
        [SerializeField]
        private bool _gizmos;

        private readonly Property<Vector3> _size = new (new Vector3(0.3f, 0.1f, 1));
        
        protected BoxCollider _collider;
        protected Rigidbody _rigidbody;
        protected Vector3[] _pathPoints;

        private void Awake()
        {
            gameObject.layer = (int)DefaultLayers.Transport;
        }

        protected void OnEnable()
        {
            OnValidate();
            GetReferences();
            CreateSurface();
            _size.OnValueChanged += OnSizeChanged;
        }

        protected void OnDisable()
        {
            _size.OnValueChanged -= OnSizeChanged;
        }
        
        protected void OnValidate()
        {
            _length = Mathf.Clamp(_length, 0, Mathf.Infinity);
            _width = Mathf.Clamp(_width, 0, Mathf.Infinity);
            _height = Mathf.Clamp(_height, 0, Mathf.Infinity);
            _size.Value = new Vector3(_width, _height, _length);
            _target.OnValidate();
        }

        private void FixedUpdate()
        {
            if (_actor != null) _target.Value = _actor.Value.Value;
            _value.Value = _target.Value * _factor;
            if (Math.FastApproximately(_value.Value, 0)) return;
            MoveSurface(_value.Value);
        }

        protected void Reset()
        {
            gameObject.layer = LayerMask.NameToLayer("Transport");
            OnValidate();
            CreateSurface();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!_isDynamic) return;
            if (other.gameObject.TryGetComponent(out Payload payload))
            {
                payload.transform.parent = transform;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (!_isDynamic) return;
            if (other.gameObject.TryGetComponent(out Payload payload))
            {
                if (payload.transform.parent == transform) payload.SetParent(null);
            }
        }

        private void OnSizeChanged(Vector3 size)
        {
            _length = Mathf.Clamp(size.z, 0, Mathf.Infinity);
            _width = Mathf.Clamp(size.x, 0, Mathf.Infinity);
            _height = Mathf.Clamp(size.y, 0, Mathf.Infinity);
            _size.SetWithoutNotify(new Vector3(_width, _height, _length));
            CreateSurface();
        }

        protected abstract void CreateSurface();
        protected abstract void MoveSurface(float speed);
        protected abstract void DrawSurface();
        public abstract Vector3 GetClosetPoint(Vector3 point);
        public abstract Vector3 GetDirection(Vector3 point);

        private void GetReferences()
        {
            if (_collider == null)
            {
                _collider = GetComponent<BoxCollider>();
                _collider.isTrigger = false;
            }

            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
                _rigidbody.isKinematic = true;
            }
        }

        private void OnDrawGizmos()
        {
            if (!_gizmos) return;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_pathPoints[0], 0.01f);
            Gizmos.DrawSphere(_pathPoints[^1], 0.01f);
            
            Gizmos.color = Color.white;
            for (var i = 1; i < _pathPoints.Length; i++)
            {
                Gizmos.DrawLine(_pathPoints[i], _pathPoints[i-1]);
            }
            
            DrawSurface();
        }
    }
}
