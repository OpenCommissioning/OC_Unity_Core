using System.Collections.Generic;
using UnityEngine;

namespace OC.MaterialFlow
{
    [SelectionBase]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    [AddComponentMenu("Open Commissioning/Material Flow/Gripper")]
    [System.Serializable]
    public class Gripper : GripperBase
    {
        protected override void PickPayloadsAction(List<Payload> payloads)
        {
            foreach (var payload in payloads)
            {
                payload.SetParent(transform);
                payload.PhysicState.Value = PhysicState.Parent;
            }
        }

        protected override void PlacePayloadsAction(List<Payload> payloads, PayloadBase parent)
        {
            var parentTransform = parent == null ?  null : parent.transform;
            var list = new List<Payload>(payloads);

            foreach (var payload in list)
            {
                payload.SetParent(parentTransform);
            }
        }
    }
}
