using System.Collections.Generic;
using UnityEngine;

namespace OC.MaterialFlow
{
    [SelectionBase]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    [AddComponentMenu("Open Commissioning/Material Flow/Gripper Fixed Joint")]
    [System.Serializable]
    public class GripperFixedJoint : GripperBase
    {
        //[SerializeField]
        //private float _breakForce = 1e6f;
        
        protected override void PickPayloadsAction(List<Payload> payloads)
        {
            throw new System.NotImplementedException();
            //TODO
            // foreach (var entity in GrippedEntities)
            // {
            //
            //     FixedJoint fj = gameObject.AddComponent<FixedJoint>();
            //     fj.connectedBody = entity.gameObject.GetComponent<Rigidbody>();
            //     fj.breakForce = _breakForce;
            // }
        }

        protected override void PlacePayloadsAction(List<Payload> payloads, PayloadBase parent)
        {
            throw new System.NotImplementedException();
            //TODO
            /*foreach (FixedJoint fixedJoint in GetComponents<FixedJoint>())
            {
                Entity connectedEntity = fixedJoint.connectedBody.GetComponent<Entity>();
                if (connectedEntity != null)
                {
                    if (isTargetNotNull)
                    {
                        connectedEntity.transform.SetParent(target.transform);
                        connectedEntity.PhysicState = PhysicState.Parent;

                    }
                    else
                    {
                        connectedEntity.transform.SetParent(Pool.Instance.transform);
                        connectedEntity.PhysicState = PhysicState.Free;
                    }
                    Destroy(fixedJoint);  
                }
            }*/
        }
    }
}
