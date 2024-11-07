using System;
using OC.MaterialFlow;
using UnityEngine;

namespace OC.Data
{
    [Serializable]
    public struct PayloadDescription
    {
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public int ControlState
        {
            get => _controlState;
            set => _controlState = value;
        }

        public int PhysicState
        {
            get => _physicState;
            set => _physicState = value;
        }

        public int Type
        {
            get => _type;
            set => _type = value;
        }

        public int GroupId
        {
            get => _groupId;
            set => _groupId = value;
        }

        public int TypeId
        {
            get => _typeId;
            set => _typeId = value;
        }

        public ulong UniqueId
        {
            get => _uniqueId;
            set => _uniqueId = value;
        }

        public Matrix4x4 Transform
        {
            get => _transform;
            set => _transform = value;
        }

        public ulong ParentUniqueId
        {
            get => _parentUniqueId;
            set => _parentUniqueId = value;
        }

        [SerializeField]
        private string _name;
        [SerializeField]
        private int _controlState;
        [SerializeField]
        private int _physicState;
        [SerializeField]
        private int _type;
        [SerializeField]
        private int _groupId;
        [SerializeField]
        private int _typeId;
        [SerializeField]
        private ulong _uniqueId;
        [SerializeField] 
        private Matrix4x4 _transform;
        [SerializeField] 
        private ulong _parentUniqueId;
        
        public PayloadDescription(Payload payload)
        {
            _name = payload.name;
            _controlState = (int)payload.ControlState.Value;
            _physicState = (int)payload.PhysicState.Value;
            _type = (int)payload.Category;
            _groupId = payload.GroupId;
            _typeId = payload.TypeId;
            _uniqueId = payload.UniqueId;
            _transform = payload.transform.GetMatrix();
            var parentPayload = payload.GetParentPayload();
            _parentUniqueId = parentPayload == null ? 0 : parentPayload.UniqueId;
        }
    }
}