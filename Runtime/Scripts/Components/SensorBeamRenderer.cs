using UnityEngine;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Sensor/Sensor Beam")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class SensorBeamRenderer : MonoBehaviour
    {
        [SerializeField]
        private Color _colorInit = Color.green;
        [SerializeField]
        private Color _colorActive = Color.red;

        private bool _isSensorValid;
        private ISensorBeam _sensor;
        private LineRenderer _lineRenderer;
        private const float DIAMETER = 0.002f;
        private float _length;

        private void OnEnable()
        {
            GetReferences();
            AddLineRenderer();
            if (!_isSensorValid) return;
            OnStateChanged(_sensor.State.Value);
            OnLengthChanged(_sensor.Length.Value);
            _sensor.State.ValueChanged += OnStateChanged;
            _sensor.Length.ValueChanged += OnLengthChanged;
        }
        
        private void OnDisable()
        {
            if (!_isSensorValid) return;
            _sensor.State.ValueChanged -= OnStateChanged;
            _sensor.Length.ValueChanged -= OnLengthChanged;
        }

        private void OnStateChanged(bool value)
        {
            SetColor(value ? _colorActive : _colorInit);
        }
        
        private void OnLengthChanged(float value)
        {
            _length = value;
            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, Vector3.forward * _length);
        }

        private void GetReferences()
        {
            _isSensorValid = gameObject.TryGetComponent(out _sensor);
            if (!_isSensorValid) Logging.Logger.Log(LogType.Error, "GameObject hasn't component with ISensorBeam interface", this);
        }

        private void OnDrawGizmos()
        {
            GetReferences();
            if (!_isSensorValid) return;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = _sensor.State.Value ? _colorActive : _colorInit;
            Gizmos.DrawSphere(Vector3.zero, DIAMETER);
            var endPoint = Vector3.forward * _length;
            Gizmos.DrawSphere(endPoint, DIAMETER);
            Gizmos.DrawLine(Vector3.zero, endPoint);
        }

        private void AddLineRenderer()
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = false;
            _lineRenderer.enabled = true;
            _lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));
            _lineRenderer.positionCount = 2;
            _lineRenderer.startWidth = DIAMETER;
            _lineRenderer.endWidth = DIAMETER;
            _lineRenderer.SetPosition(0, Vector3.zero);
            _lineRenderer.SetPosition(1, Vector3.forward * _length);
        }

        private void SetColor(Color color)
        {
            _lineRenderer.startColor = color;
            _lineRenderer.endColor = color;
        }
    }
}
