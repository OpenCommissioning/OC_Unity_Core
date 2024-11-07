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
    public abstract class Drive : Actor, IDeviceMetadata, IControlOverridable, ICustomInspector, IInteractable
    {
        public Link Link => _link;
        public int MetadataAssetLength => 1;

        public IProperty<bool> IsActive => _isActive;

        [SerializeField]
        protected Property<bool> _isActive = new (false);

        public UnityEvent<bool> OnActiveChanged;

        [SerializeField]
        protected Link _link;
        [SerializeField]
        protected ConnectorDataFloat _connectorData;
        
        private void Start()
        {
            _link.Initialize(this);
            _connectorData = new ConnectorDataFloat(_link);
            _isActive.ValueChanged += value => OnActiveChanged?.Invoke(value);
        }

        protected void Reset()
        {
            _link = new Link(this, "FB_Drive");
        }
        
        private void FixedUpdate()
        {
            if (!Override.Value && _link.IsConnected) GetLinkData();
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
            StartCoroutine(InitilizeCoroutine(value));
            Logging.Logger.Log(LogType.Log, "Device state initialized from metadata", this);
        }
        
        private IEnumerator InitilizeCoroutine(float value)
        {
            _value.Value = value;
            if (!_link.IsConnected)
            {
                Logging.Logger.Log(LogType.Warning, "Device initialization sequence is cancelled! Simulation unit communication isn't active!", this);
                yield break;
            }

            _connectorData.Status.SetBit(7, true);
            _connectorData.StatusData = value;
            yield return new WaitForSeconds(1);
            _connectorData.Status.SetBit(7, false);
        }
    }
}
