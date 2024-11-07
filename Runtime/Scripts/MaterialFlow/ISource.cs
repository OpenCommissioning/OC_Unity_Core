using UnityEngine;

namespace OC.MaterialFlow
{
    public interface ISource
    {        
        public Property<int> TypeId { get; }
        public Property<ulong> UniqueId { get; }
        // ReSharper disable once InconsistentNaming
        public GameObject gameObject { get; }
        public void Create();
    }    
}
