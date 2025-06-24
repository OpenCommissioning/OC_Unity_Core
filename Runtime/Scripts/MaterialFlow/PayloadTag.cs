using System.Collections.Generic;
using OC.Data;
using UnityEngine;

namespace OC.MaterialFlow
{
    [DefaultExecutionOrder(1000)]
    [RequireComponent(typeof(Payload))]
    [DisallowMultipleComponent]
    public class PayloadTag : MonoBehaviour
    {
        public Payload Payload => _payload;
        public List<int> DirectoryId => _directoryId;
        
        [SerializeField]
        private List<int> _directoryId = new();
        private Payload _payload;
        
        private void OnEnable()
        {
            _payload = GetComponent<Payload>();
            this.CreateProductData();
        }

        public ulong Read()
        {
            return _payload == null ? 0 : _payload.UniqueId;
        }
    }
}