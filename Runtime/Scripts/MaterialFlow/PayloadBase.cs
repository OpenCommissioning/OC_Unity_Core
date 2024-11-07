using UnityEngine;

namespace OC.MaterialFlow
{
    public abstract class PayloadBase: MonoBehaviour
    {
        public abstract int GroupId { get; set; }
        public event System.Action OnDestroyAction;
        public event System.Action OnDisableAction;
        
        protected void OnDisable()
        {
            OnDisableAction?.Invoke();
        }

        protected void OnDestroy()
        {
            OnDestroyAction?.Invoke();
        }

        public void DestroyDirty()
        {
            gameObject.Destroy();
            OnDestroyAction?.Invoke();
        }
    }
}
