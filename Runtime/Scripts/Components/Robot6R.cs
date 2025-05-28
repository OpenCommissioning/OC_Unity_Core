using System.Collections.Generic;
using OC.Communication;
using UnityEngine;

namespace OC.Components
{
    [AddComponentMenu("Open Commissioning/Actor/Robot 6R")]
    public class Robot6R : MonoComponent, IDevice
    {
        public Link Link => _link;
        public float[] Target => _target;

        [Header("Control")]
        [SerializeField] 
        private float[] _target = new float[12];
        
        [Header("SceneConfigurationManager")]
        [SerializeField] 
        private float[] _factor = new float[12];
        [SerializeField] 
        private float[] _offset = new float[12];
        
        [Header("References")]
        [SerializeField]
        private List<Axis> _axes;
        
        [SerializeField]
        private Link _link;
        [SerializeField]
        private ConnectorDataRobot _connector;

        private readonly float[] _value = new float[12];
        
        private void Start()
        {
            _link.Initialize(this);
            _connector = new ConnectorDataRobot(_link);
        }
        
        private void Reset()
        {
            _link = new Link(this, "FB_Robot");
        }

        private void FixedUpdate()
        {
            if (_link.IsActive) _connector.JointTarget.CopyTo(_value, 0);

            for (var i = 0; i < _value.Length; i++)
            {
                _target[i] = (_value[i] + _offset[i]) * _factor[i];
            }

            for (var i = 0; i < _axes.Count; i++)
            {
                _axes[i].Target.Value =  _target[i];
            }
        }
    }
}
