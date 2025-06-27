using System.Collections.Generic;
using OC.Communication;
using UnityEngine;

namespace OC.Components
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [AddComponentMenu("Open Commissioning/Actor/Robot 6R")]
    public class Robot6R : MonoComponent, IDevice
    {
        public Link Link => _link;
        public IProperty<bool> Override => _override;
        public float[] Target => _target;

        [Header("Control")]
        [SerializeField] 
        private float[] _target = new float[12];
        
        [Header("Settings")]
        [SerializeField] 
        private float[] _factor = new float[12];
        [SerializeField] 
        private float[] _offset = new float[12];
        
        [Header("References")]
        [SerializeField]
        private List<Axis> _axes;
        
        [SerializeField]
        protected Property<bool> _override = new (false);
        [SerializeField]
        private LinkDataRobot _link = new("FB_Robot");

        private readonly float[] _value = new float[12];
        
        private void Start()
        {
            _link.Initialize(this);
        }

        private void FixedUpdate()
        {
            if (_link.Connected) _link.JointTarget.CopyTo(_value, 0);

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
