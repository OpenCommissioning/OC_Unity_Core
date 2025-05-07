using UnityEngine;

namespace OC.MaterialFlow
{    
    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    public class Pool : MonoBehaviourSingleton<Pool>
    {
        public PoolManager PoolManager => _poolManager;

        [SerializeField]
        private PoolManager _poolManager = new ();
        
        public override void Awake()
        {
            base.Awake();
            if (_poolManager.Root == null) _poolManager.Root = transform;
        }

        
        public void DestroyAll()
        {
            _poolManager?.DestroyAll();
        }
        
        public void Repair()
        {
            _poolManager?.Repair();
        }
    }
}
