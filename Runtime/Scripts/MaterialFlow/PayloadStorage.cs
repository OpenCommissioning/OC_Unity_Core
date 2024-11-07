using UnityEngine;
using UnityEngine.Events;

namespace OC.MaterialFlow
{
    [SelectionBase]
    [RequireComponent(typeof(BoxCollider))]
    [DisallowMultipleComponent]
    public class PayloadStorage : PayloadBase
    {
        public override int GroupId
        {
            get => _groupId;
            set => _groupId = value;
        }

        [SerializeField]
        private int _groupId;

        public UnityEvent<GameObject> OnChildrenAdd;

        public void Add(PayloadBase payload)
        {
            payload.transform.parent = transform;
        }

        public void DestroyAll()
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(transform.childCount - 1).Destroy();
            }
        }

        private void OnTransformChildrenChanged()
        {
            if (transform.childCount > 0)
            {
                OnChildrenAdd?.Invoke(transform.GetChild(transform.childCount - 1).gameObject);
            }
        }
    }
}
