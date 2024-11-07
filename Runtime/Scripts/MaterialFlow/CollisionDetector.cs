using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OC.MaterialFlow
{
    [Serializable]
    public class CollisionDetector
    {
        public CollisionFilter Filter
        {
            get => _filter;
            set => _filter = value;
        }

        public int GroupId
        {
            get => _groupId;
            set => _groupId = value;
        }

        public List<PayloadBase> Buffer => _buffer;

        public Property<bool> Collision => _collision;

        [SerializeField]
        private CollisionFilter _filter;
        [SerializeField]
        private int _groupId;
        
        private Property<bool> _collision = new (false);
        private List<PayloadBase> _buffer = new ();
        
        public Action<PayloadBase> PayloadEnterAction;
        public Action<PayloadBase> PayloadExitAction;

        public CollisionDetector(){}
        
        public CollisionDetector(CollisionFilter filter, int groupId)
        {
            _filter = filter;
            _groupId = groupId;
        }
        
        public bool Add(GameObject gameObject)
        {
            return gameObject.TryGetComponent(out PayloadBase payloadObject) && Add(payloadObject);
        }

        public bool Add(PayloadBase payloadBase)
        {
            if (!IsTypeValid(payloadBase, _filter)) return false;
            if (!IsGroupValid(payloadBase.GroupId, _groupId)) return false;
            if (_buffer.Contains(payloadBase)) return false;

            payloadBase.OnDestroyAction += () => Remove(payloadBase);
            payloadBase.OnDisableAction += () => Remove(payloadBase);
            _buffer.Add(payloadBase);
            
            PayloadEnterAction?.Invoke(payloadBase);
            Refresh();
            return true;
        }

        public bool Remove(PayloadBase payloadBase)
        {
            if (!IsTypeValid(payloadBase, _filter) || !IsGroupValid(payloadBase.GroupId, _groupId)) return false;
            if (!_buffer.Contains(payloadBase)) return false;
            _buffer.Remove(payloadBase);
            PayloadExitAction?.Invoke(payloadBase);
            Refresh();
            return true;
        }

        public bool Remove(GameObject gameObject)
        {
            return gameObject.TryGetComponent(out PayloadBase payloadObject) && Remove(payloadObject);
        }

        public void ClearAll()
        {
            _buffer.Clear();
            Refresh();
        }

        public void DestroyAll()
        {
            var destroyList = new List<PayloadBase>(_buffer);
            
            foreach (var payload in destroyList)
            {
                if (!Application.isPlaying)
                {
                    Object.DestroyImmediate(payload.gameObject);
                }
                else
                {
                    Object.Destroy(payload.gameObject);
                }
            }
            ClearAll();
        }
        
        public static bool IsTypeValid(PayloadBase payloadBase, CollisionFilter filter)
        {
            return payloadBase switch
            {
                PayloadStorage => filter.HasFlag(CollisionFilter.Storage),
                StaticCollider => filter.HasFlag(CollisionFilter.Static),
                Payload payload => ((int)filter & 2.Pow((int)payload.Category)) != 0,
                _ => false
            };
        }

        public static bool IsGroupValid(int groupId, int requiredGroupId)
        {
            if (requiredGroupId == 0) return true;
            return requiredGroupId == groupId;
        }
        
        private void Refresh()
        {
            _collision.Value = _buffer.Count > 0;
        }

        public PayloadStorage GetLastPayloadStorage()
        {
            for (var i = _buffer.Count - 1; i >= 0; i--)
            {
                if (_buffer[i] is not PayloadStorage e) continue;
                return e;
            }
            return null;
        }

        public Payload GetLastByType(Payload.PayloadCategory type)
        {
            for (var i = _buffer.Count - 1; i >= 0; i--)
            {
                if (_buffer[i] is not Payload e) continue;
                if (e.Category == type)
                {
                    return e;
                }
            }
            return null;
        }
    }
    
    [Flags]
    public enum CollisionFilter
    {
        None = 0,
        Part = 1,
        Assembly = 2,
        Transport = 4,
        Static = 8,
        Storage = 16,
        All = ~0
    }
}
