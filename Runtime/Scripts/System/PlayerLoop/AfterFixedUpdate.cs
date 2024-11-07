using System.Collections.Generic;
using UnityEngine;

namespace OC.PlayerLoop
{
    public static class AfterFixedUpdateSystem
    {
        public static void Register(IAfterFixedUpdate item) => Items.Add(item);
        public static void Unregister(IAfterFixedUpdate item) => Items.Remove(item);
        private static readonly HashSet<IAfterFixedUpdate> Items = new();
        
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() 
        {
            PlayerLoopInterface.InsertSystemAfter(typeof(AfterFixedUpdateSystem), AfterFixedUpdate, typeof(UnityEngine.PlayerLoop.FixedUpdate.ScriptRunBehaviourFixedUpdate));
        }

        private static void AfterFixedUpdate() 
        {
            using var enumerator = Items.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current?.AfterFixedUpdate();
            }
        }
    }

    public interface IAfterFixedUpdate
    {
        public void AfterFixedUpdate();
    }
}