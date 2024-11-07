using UnityEngine;

namespace OC
{
    public class SceneCamera : MonoBehaviour
    {
        public static SceneCamera Instance { get; private set; }
        
        public float Sensitivity
        {
            set => _sensitivity = value;
        }

        public float MoveSpeed
        {
            set => _moveSpeed = value * _moveSpeedFactor * 0.01f;
        }

        [Header("Focus Object")] 
        [SerializeField] private GameObject _focusObject;
        [SerializeField, Tooltip("Enable double-click to focus on objects?")]
        private bool _doFocus;
        [SerializeField] private KeyCode _focusKey = KeyCode.F;
        [SerializeField] private float _focusLimit = 100f;
        [SerializeField] private float _minFocusDistance = 5.0f;

        [Header("Undo - Only undoes the Focus Object - The keys must be pressed in order.")] [SerializeField]
        private KeyCode _firstUndoKey = KeyCode.LeftControl;

        [SerializeField] private KeyCode _secondUndoKey = KeyCode.Z;

        [Header("Movement")] 
        [SerializeField] private float _sensitivity = 1f;
        [SerializeField] private float _moveSpeedFactor = 1f;
        [SerializeField] private float _moveSpeed = 0.01f;
        [SerializeField] private float _rotationSpeed = 1f;
        [SerializeField] private float _zoomSpeed = 0.1f;

        [Header("Slave Names")] [SerializeField, Tooltip("Otherwise known as the vertical axis")]
        private string _mouseY = "Mouse Y";

        [SerializeField, Tooltip("AKA horizontal axis")]
        private string _mouseX = "Mouse X";

        [SerializeField, Tooltip("The axis you want to use for zoom.")]
        private string _zoomAxis = "Mouse ScrollWheel";

        [Header("Operation Keys")] [SerializeField] private KeyCode _forwardKey = KeyCode.W;
        [SerializeField] private KeyCode _backKey = KeyCode.S;
        [SerializeField] private KeyCode _leftKey = KeyCode.A;
        [SerializeField] private KeyCode _rightKey = KeyCode.D;

        [Header("Flat Operation"), Tooltip("Instead of going where the camera is pointed, the camera moves only on the horizontal plane (Assuming you are working in 3D with default preferences).")]
        [SerializeField]
        private KeyCode _flatMoveKey = KeyCode.LeftShift;

        [Header("Anchored Movement"),
         Tooltip(
             "By default in scene-view, this is done by right-clicking for rotation or middle mouse clicking for up and down")]
        [SerializeField]
        private KeyCode _anchoredMoveKey = KeyCode.Mouse2;

        [SerializeField] private KeyCode _anchoredRotateKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode _focusRotateKey = KeyCode.LeftAlt;

        private Vector3 _prevPosition;
        private Quaternion _prevRotation;
        private Vector3 _initPosition;
        private Quaternion _initRotation;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            } 
            else 
            {
                Instance = this;
            }
        }
        
        private void Start()
        {
            SavePosAndRot();
            _initPosition = transform.position;
            _initRotation = transform.rotation;
        }

        private void Update()
        {
            if (!_doFocus) return;
            if (Input.GetKeyDown(_focusKey)) FocusObject();
            if (Input.GetKey(_firstUndoKey))
            {
                if (Input.GetKeyDown(_secondUndoKey)) GoBackToLastPosition();
            }
            ResetTransform();
        }

        private void LateUpdate()
        {
            if (Utils.IsPointerOverUIElement()) return;

            var move = Vector3.zero;

            if (Input.GetKey(_forwardKey))
                move += Vector3.forward * _moveSpeed;
            if (Input.GetKey(_backKey))
                move += Vector3.back * _moveSpeed;
            if (Input.GetKey(_leftKey))
                move += Vector3.left * _moveSpeed;
            if (Input.GetKey(_rightKey))
                move += Vector3.right * _moveSpeed;

            if (Input.GetKey(_flatMoveKey))
            {
                var origY = transform.position.y;
                transform.Translate(move);
                transform.position = new Vector3(transform.position.x, origY, transform.position.z);
                return;
            }

            var mouseMoveY = Input.GetAxis(_mouseY);
            var mouseMoveX = Input.GetAxis(_mouseX);

            if (Input.GetKey(_anchoredMoveKey))
            {
                move += Vector3.up * (mouseMoveY * -_moveSpeed * 0.5f * _sensitivity);
                move += Vector3.right * (mouseMoveX * -_moveSpeed * 0.5f * _sensitivity);
            }

            if (Input.GetKey(_anchoredRotateKey))
            {
                if (Input.GetKey(_focusRotateKey))
                {
                    var focusPosition = _focusObject == null ? Vector3.zero : _focusObject.transform.position;
                    transform.RotateAround(focusPosition, transform.right, mouseMoveY * -(_rotationSpeed * _sensitivity));
                    transform.RotateAround(focusPosition, Vector3.up, mouseMoveX * (_rotationSpeed * _sensitivity));
                }
                else
                {
                    transform.RotateAround(transform.position, transform.right, mouseMoveY * -(_rotationSpeed * _sensitivity));
                    transform.RotateAround(transform.position, Vector3.up, mouseMoveX * (_rotationSpeed * _sensitivity));
                }
            }

            transform.Translate(move);

            var mouseScroll = Input.GetAxis(_zoomAxis);
            transform.Translate(Vector3.forward * (mouseScroll * _zoomSpeed * _sensitivity));
        }

        public void ResetToInitPosition()
        {
            StopAllCoroutines();
            StartCoroutine(transform.Interpolate(_initPosition, _initRotation, 0.5f));
        }

        public void FocusToGameObject(GameObject target)
        {
            if (!target.TryGetComponent<Collider>(out var col)) return;
            StopAllCoroutines();
            var targetPos = target.transform.position;
            var targetSize = col.bounds.size;
            var direction = targetPos - transform.position;
            var position = targetPos + GetOffset(targetPos, targetSize);
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            StartCoroutine(transform.Interpolate(position, rotation, 0.5f));
        }

        private void ResetTransform()
        {
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKey(KeyCode.R))
            {
                ResetToInitPosition();
            }
        }

        private void FocusObject()
        {
            SavePosAndRot();
            if (Camera.main == null) return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, _focusLimit))
            {
                _focusObject = hit.collider.gameObject;
                var targetPos = _focusObject.transform.position;
                var targetSize = hit.collider.bounds.size;

                transform.position = targetPos + GetOffset(targetPos, targetSize);

                transform.LookAt(_focusObject.transform);
            }
        }

        private void SavePosAndRot()
        {
            _prevRotation = transform.rotation;
            _prevPosition = transform.position;
        }

        private void GoBackToLastPosition()
        {
            transform.position = _prevPosition;
            transform.rotation = _prevRotation;
        }

        private Vector3 GetOffset(Vector3 targetPos, Vector3 targetSize)
        {
            var dirToTarget = targetPos - transform.position;
            var focusDistance = Mathf.Max(targetSize.x, targetSize.z);
            focusDistance = Mathf.Clamp(focusDistance, _minFocusDistance, focusDistance);
            return -dirToTarget.normalized * focusDistance;
        }
    }
}
