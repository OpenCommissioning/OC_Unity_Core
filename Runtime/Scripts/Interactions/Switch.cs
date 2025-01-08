using OC.Communication;
using OC.Components;
using UnityEngine;
using UnityEngine.Events;

namespace OC.Interactions
{
    public class Switch : Device
    {
        public override int AllocatedBitLength => Mathf.Max(0,_stateCount - 1);

        public IProperty<int> Index => _index;

        [Header("Switch")] 
        [SerializeField] 
        protected Property<int> _index = new (0);
        [SerializeField] 
        protected int _stateCount = 2;

        public UnityEvent<int> OnIndexChanged;

        protected void OnEnable()
        {
            _index.ValueChanged += IndexChanged;
        }
        
        protected void OnDisable()
        {
            _index.ValueChanged -= IndexChanged;
        }
        
        protected override void Reset()
        {
            _link = new Link(this, "FB_Switch");
        }

        protected void OnValidate()
        {
            _stateCount = Mathf.Clamp(_stateCount, 2, 255);
        }

        private void IndexChanged(int index)
        {
            index %= _stateCount;
            Connector.Status = index > 0 ? (byte)Mathf.Pow(2, index - 1) : (byte)0;
            OnIndexChanged?.Invoke(index);
        }

        public void Click()
        {
            if (!Application.isPlaying) return;
            
            if (_stateCount < 1)
            {
                Logging.Logger.Log(LogType.Warning,"Position count < 1",this);
                return;
            }

            _index.Value = (_index.Value + 1) % _stateCount;
        }
    }
}