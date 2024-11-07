using UnityEngine;

namespace OC
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component
    {
        public Component Component => this;
        public static T Instance { get; private set; }
	
        public virtual void Awake ()
        {
            if (Instance == null) 
            {
                Instance = this as T;
            } 
            else 
            {
                Destroy (gameObject);
            }
        }
    }
}