using UnityEngine;

namespace OC.MaterialFlow
{
    [RequireComponent(typeof(BoxCollider))]
    public class StaticCollider: PayloadBase
    {
        public override int GroupId
        {
            get => _groupId;
            set => _groupId = value;
        }
        
        [Header("Data")]
        [SerializeField] 
        private int _groupId;
    }
}
