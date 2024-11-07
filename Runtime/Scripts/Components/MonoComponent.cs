using UnityEngine;

namespace OC.Components
{
    public class MonoComponent : MonoBehaviour, IComponent
    {
        public Component Component => this;
    }
}