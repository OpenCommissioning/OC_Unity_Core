using System;
using UnityEngine;

namespace OC
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }
        
        public Property<float> TimeScale => _timeScale;
        public Property<bool> Pause => _pause;

        [SerializeField]
        private Property<float> _timeScale = new(1);
        [SerializeField] 
        private Property<bool> _pause = new();

        private float _time;
        public event Action OnSecondTick;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void RuntimeInit()
        {
            if (Instance != null) return;
            
            var gameObject = new GameObject { name = "[Time Manager]" };
            Instance = gameObject.AddComponent<TimeManager>();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _timeScale.Validator = TimeScaleValidator;
            _timeScale.OnValueChanged += TimeScaleChanged;
            _pause.OnValueChanged += PauseChanged;
        }

        private void OnDisable()
        {
            _timeScale.OnValueChanged -= TimeScaleChanged;
            _pause.OnValueChanged -= PauseChanged;
        }

        private void Start()
        {
            Time.timeScale = _pause.Value ? 0 : _timeScale.Value;
        }
        
        private void OnValidate()
        {
            _timeScale.OnValidate();
            _pause.OnValidate();
        }

        private void Update()
        {
            SecondTick();
        }

        private void TimeScaleChanged(float value)
        {
            RefreshTimeScale();
        }
        
        private void PauseChanged(bool value)
        {
            RefreshTimeScale();
        }
        
        private float TimeScaleValidator(float value)
        {
            return Mathf.Clamp(value, 0.001f, 100);
        }

        private void RefreshTimeScale()
        {
            Time.timeScale = _pause.Value ? 0 : _timeScale.Value;
        }

        private void SecondTick()
        {
            _time += Time.deltaTime;
            if (!(_time > 1)) return;
            _time -= 1;
            OnSecondTick?.Invoke();
        }
    }
}
