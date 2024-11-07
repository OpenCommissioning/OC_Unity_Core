using System.Collections.Generic;
using UnityEngine;

namespace OC.PlayerLoop
{
    public static class BeforeFixedUpdateSystem
    {
        public static void Register(IBeforeFixedUpdate item) => Items.Add(item);
        public static void Unregister(IBeforeFixedUpdate item) => Items.Remove(item);
        private static readonly HashSet<IBeforeFixedUpdate> Items = new();
        
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() 
        {
            PlayerLoopInterface.InsertSystemBefore(typeof(BeforeFixedUpdateSystem), BeforeFixedUpdate, typeof(UnityEngine.PlayerLoop.FixedUpdate.ScriptRunBehaviourFixedUpdate));
        }

        private static void BeforeFixedUpdate() 
        {
            using var enumerator = Items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current?.BeforeFixedUpdate();
            }
        }
    }
    
    public interface IBeforeFixedUpdate
    {
        public void BeforeFixedUpdate();
    }
}