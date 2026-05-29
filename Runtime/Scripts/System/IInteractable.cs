using System;
using UnityEngine;

namespace OC
{
    public interface IInteractable
    {
        public Type ReferenceType { get; }
        public Component Component { get; }
    }
}
