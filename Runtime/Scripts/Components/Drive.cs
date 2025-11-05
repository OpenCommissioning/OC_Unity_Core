using System.Collections;
using System.Globalization;
using OC.Communication;
using OC.Data;
using UnityEngine;
using UnityEngine.Events;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Actor/Drive")]
    [SelectionBase]
    [DisallowMultipleComponent]
    public abstract class Drive : Actor, IDevice, IMetadataAsset, ICustomInspector, IInteractable
    {
        public Link Link => _link;
        public IProperty<bool> Override => _override;
        public int MetadataAssetLength => 1;

        public IPropertyReadOnly<bool> IsActive => _stateObserver.IsActive;
        public IPropertyReadOnly<DriveState> State => _stateObserver.State;

        public UnityEvent<bool> OnActiveChanged;

        [SerializeField]
        protected Property<bool> _override = new (false);
        [SerializeField]
        protected LinkDataFloat _link = new("FB_Drive");
        [HideInInspector]
        [SerializeField]
        protected DriveStateObserver _stateObserver = new ();

        protected void OnEnable()
        {
            _stateObserver.IsActive.Subscribe(OnActiveChanged.Invoke);
        }
        
        protected void OnDisable()
        {
            _stateObserver.IsActive.Unsubscribe(OnActiveChanged.Invoke);
        }
        
        protected void Start()
        {
            _link.Initialize(this);
        }
        
        protected void FixedUpdate()
        {
            if (!_override && _link.Connected) GetLinkData();
            Operation(Time.fixedDeltaTime);
            SetLinkData();
        }

        protected abstract void GetLinkData();
        protected abstract void SetLinkData();
        protected abstract void Operation(float deltaTime);

        public MetadataAsset GetAsset()
        {
            var asset = new MetadataAsset("DriveData");
            asset.Data.Add(new DictionaryItem("Value", _value.Value.ToString(CultureInfo.InvariantCulture)));
            return asset;
        }

        public void SetAsset(MetadataAsset asset)
        {
            var metadataItem = asset.Data.Find(item => item.Key == "Value");

            if (metadataItem is null) return;
            if (!int.TryParse(metadataItem.Value, out var value)) return;
            
            Target.Value = value;
            StopAllCoroutines();
            StartCoroutine(InitializeCoroutine(value));
            Logging.Logger.Log(LogType.Log, "Device state initialized from metadata", this);
        }
        
        private IEnumerator InitializeCoroutine(float value)
        {
            _value.Value = value;
            if (!_link.Connected)
            {
                Logging.Logger.Log(LogType.Warning, "Device initialization sequence is cancelled! Simulation unit communication isn't active!", this);
                yield break;
            }

            _link.Status.SetBit(7, true);
            _link.StatusData = value;
            yield return new WaitForSeconds(1);
            _link.Status.SetBit(7, false);
        }
        
        public enum DriveState
        {
            Idle,
            IsRunningNegative,
            IsRunningPositive
        }
    }
}
