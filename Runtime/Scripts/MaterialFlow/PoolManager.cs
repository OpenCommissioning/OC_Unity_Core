using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using OC.Data;

namespace OC.MaterialFlow
{
    [Serializable]
    public class PoolManager
    {
        public Transform Root 
        {
            get => _root;
            set => _root = value;
        }
        
        public List<Payload> PayloadList
        {
            get => _payloadList;
            set => _payloadList = value;
        }
        
        public Dictionary<ulong, Payload> Payloads => _payloads;
        
        public bool Verbose 
        {
            get => _verbose;
            set => _verbose = value;
        }
        
        public int Count => _count;


        [SerializeField]
        private Transform _root;
        [SerializeField]
        private List<Payload> _payloadList = new ();
        [SerializeField] 
        private bool _verbose;
        [SerializeField] 
        private int _count;

        private readonly Dictionary<ulong, Payload> _payloads;
        private const string TAG_CREATE = "Create";
        private const string TAG_DESTROY = "Destroy";
        private const string TAG_REGISTRATE = "Registrate";
        private const string TAG_UNREGISTRATE = "Unregistrate";
        private const string TAG_REPLACE = "Replace";

        public PoolManager()
        {
            _payloads = new Dictionary<ulong, Payload>();
        }

        public PoolManager(Transform root, List<Payload> prefabs)
        {
            _root = root;
            _payloads = new Dictionary<ulong, Payload>();
            _payloadList = prefabs;
        }

        public bool IsTypeValid(int type)
        {
            return type >= 0 && type < _payloadList.Count;
        }

        public Payload Instantiate(Vector3 position, Quaternion rotation, int typeId, ulong uniqueId = 0, Object context = null)
        {
            try
            {
                if (!IsTypeValid(typeId))
                {
                    throw new Exception($"TypeId: {typeId} isn't valid!");
                }
                
                if (uniqueId == 0) uniqueId = GetNextFreeIndex(_payloads.Keys.ToList());
                if (uniqueId != 0 && _payloads.ContainsKey(uniqueId))
                {
                    throw new Exception($"UniqueId: {uniqueId} already exists!");
                }
                
                var payload = InstantiateDirty(position, rotation, typeId, uniqueId);

                Registrate(payload);
                Logging.Logger.Log(LogType.Log, $"{TAG_CREATE} {payload}", payload);
                return payload;
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, $"{TAG_CREATE} {exception.Message}", context);
                return null;
            }
        }
        
        public Payload Instantiate(ISource source)
        {
            return Instantiate(source.gameObject.transform.position, source.gameObject.transform.rotation, source.TypeId, source.UniqueId, context: source.gameObject);
        }
        
        public Payload Instantiate(ISource source, ulong uniqueId)
        {
            return Instantiate(source.gameObject.transform.position, source.gameObject.transform.rotation, source.TypeId, uniqueId, context: source.gameObject);
        }
        
        public Payload Instantiate(PayloadDescription description)
        {
            var payload = Instantiate(description.Transform.GetPosition(), description.Transform.rotation, description.TypeId, description.UniqueId);
            if (payload is null) return null;
            
            payload.ApplyDiscription(description);
            
            var parent = FindPayload(description.ParentUniqueId);
            if (parent is not null)
            {
                payload.SetParent(parent.transform);
            }

            return payload;
        }

        public (bool, ulong) ChangeRegistration(Payload payload, ulong uniqueId)
        {
            try
            {
                if (payload == null) throw new Exception("Payload is null!");
                if (uniqueId == 0) throw new Exception($"{payload} UniqueId is null!");
                if (_payloads.ContainsKey(uniqueId)) throw new Exception($"{payload} already exists!");
                Unregistrate(payload);
                payload.Unregistrate(uniqueId);
                return Registrate(payload);
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, $"{TAG_REGISTRATE} {exception.Message}", payload);
                return (false, payload.UniqueId);
            }
        }

        public (bool, ulong) Registrate(Payload payload)
        {
            return Registrate(payload, payload.UniqueId);
        }
        
        public (bool, ulong) Registrate(Payload payload, ulong uniqueId)
        {
            try
            {
                if (payload == null)
                {
                    throw new Exception("Payload is null!");
                }
                
                if (payload.IsRegistered) return (true, payload.UniqueId);
                
                if (!IsTypeValid(payload.TypeId))
                {
                    throw new Exception($"{payload} TypeId isn't valid!");
                }
                
                if (uniqueId != 0 && _payloads.ContainsKey(uniqueId))
                {
                    throw new Exception($"UniqueId {uniqueId} already exists!");
                }

                if (uniqueId == 0) uniqueId = GetNextFreeIndex(_payloads.Keys.ToList());

                payload.Registrate(uniqueId);
                AddToPool(uniqueId, payload);

                for (var i = 0; i <  payload.transform.childCount; i++)
                {
                    if (payload.transform.GetChild(i).TryGetComponent(out Payload childEntity))
                    {
                        Registrate(childEntity); 
                    }
                }

                return (true, uniqueId);
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, $"{TAG_REGISTRATE} {exception.Message}", payload);
                return (false, payload.UniqueId);
            }
        }
        
        public void Unregistrate(Payload payload)
        {
            try
            {
                if (payload is null)
                {
                    throw new Exception($"{TAG_UNREGISTRATE} Payload is null!");
                }
                
                if (!payload.IsRegistered) return;

                if (!_payloads.ContainsKey(payload.UniqueId))
                {
                    var message = $"UniqueId: {payload.UniqueId} isn't contains in pool! Data was manipulated without permission!";
                    payload.Unregistrate();
                    throw new Exception($"{TAG_UNREGISTRATE} {message}");
                }
                
                RemoveFromPool(payload.UniqueId);
                for (var i = 0; i <  payload.transform.childCount; i++)
                {
                    if (payload.transform.GetChild(i).TryGetComponent(out Payload childEntity))
                    {
                        Unregistrate(childEntity); 
                    }
                }
                payload.Unregistrate();
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, exception.Message, payload);
                throw;
            }
        }
        
        public void Destroy(Payload payload, float delay = 0f)
        {
            try
            {
                if (payload is null)
                {
                    throw new Exception("Payload is null!");
                }
                
                Unregistrate(payload);

                var entityString = payload.ToString();
                
                if (Application.isPlaying)
                {
                    Object.Destroy(payload.gameObject, delay);
                }
                else
                {
                    Object.DestroyImmediate(payload.gameObject);
                }
                
                Logging.Logger.Log(LogType.Log, $"{TAG_DESTROY} {entityString}" , _root);
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, $"{TAG_DESTROY} {exception.Message}", _root);
            }
        }
        
        public Payload Replace(Object source, Payload payload, int typeId)
        {
            try
            {
                if (payload is null)
                {
                    throw new Exception("Payload is null!");
                }
                
                if (typeId == payload.TypeId)
                {
                    throw new Exception($"TypeId {payload.TypeId} is equal!");
                }

                if (!IsTypeValid(typeId))
                {
                    throw new Exception($"TypeId {typeId} as target isn't valid!");
                }

                var lastTypeId = payload.TypeId;
                var newPayload = ReplacePrefab(payload, typeId);
                var message = $"{newPayload.name} [UniqueId: {newPayload.UniqueId}, TypeId: {lastTypeId}] to [UniqueId: {newPayload.UniqueId}, TypeId: {newPayload.TypeId}]";
                Logging.Logger.Log(LogType.Log, $"{TAG_REPLACE} {message}", source);
                return newPayload;
            }
            catch (Exception e)
            {
                Logging.Logger.Log(LogType.Error, $"{TAG_REPLACE} {e.Message}", source);
                return null;
            }
        }

        public int GetValidType(int type)
        {
            return _payloadList.Count > 0 ? Mathf.Clamp(type, 0, _payloadList.Count - 1) : -1;
        }

        public void DestroyAll()
        {
            if (_payloads == null) return;
            if (_payloads.Count == 0) return;
            foreach (var item in _payloads.Values.Reverse())
            {
                Destroy(item);
            }
            _count = Payloads.Count;
        }

        public void Repair()
        {
            if (_payloads == null) return;
            if (_payloads.Count == 0) return;

            var invalidKeys = _payloads.Where(item => item.Value == null).Select(item => item.Key).ToList();
            foreach (var key in invalidKeys)
            {
                _payloads.Remove(key);
                Logging.Logger.Log(LogType.Warning, $"UniqueId {key} is not valid. Is deleted from the pool registry");
            }
            _count = Payloads.Count;
            Logging.Logger.Log(LogType.Log, "Repair is completed!");
        }

        public ulong GetPossibleUniqueId(ulong uniqueId)
        {
            return IsUniqueIdValid(uniqueId) ? uniqueId : GetNextFreeIndex(_payloads.Keys.ToList());
        }

        public bool IsUniqueIdValid(ulong uniqueId)
        {
            return uniqueId == 0 || !_payloads.ContainsKey(uniqueId);
        }
        
        private Payload InstantiateDirty(Vector3 position, Quaternion rotation, int typeId, ulong uniqueId)
        {
            var parent = new GameObject();
            parent.SetActive(false);
            var newObject = Object.Instantiate(_payloadList[typeId].gameObject, position, rotation, parent.transform).GetComponent<Payload>();
            var payload = newObject.GetComponent<Payload>();
            var name = payload.name.Replace("(Clone)", "");
            payload.name = $"{name}_{uniqueId}";
            payload.Unregistrate(uniqueId);
            payload.ControlState.Value = ControlState.Ready;
            
            newObject.transform.parent = _root != null ? _root : null;
            Object.DestroyImmediate(parent);
            return payload;
        }
        
        private Payload ReplacePrefab(Payload payload, int typeId)
        {
            Unregistrate(payload);
            var newPayload = InstantiateDirty(payload.transform.position, payload.transform.rotation, typeId, payload.UniqueId);
            newPayload.name = payload.name;
            newPayload.ControlState.Value = payload.ControlState.Value;
            newPayload.PhysicState.Value = payload.PhysicState.Value;
            newPayload.Unregistrate(payload.UniqueId);
            Registrate(newPayload);
            Object.DestroyImmediate(payload.gameObject);
            return newPayload;
        }

        private void AddToPool(ulong uniqueId, Payload payload)
        {
            _payloads.Add(uniqueId, payload);
            _count = Payloads.Count;
            if (!Verbose) return;
            var message = $"{TAG_REGISTRATE} {payload}]";
            Logging.Logger.LogVerbose(LogType.Log, message, payload);
        }

        private void RemoveFromPool(ulong uniqueId)
        {
            _payloads.Remove(uniqueId);
            _count = Payloads.Count;
            if (!Verbose) return;
            var message = $"{TAG_UNREGISTRATE} Payload [UniqueId: {uniqueId}]";
            Logging.Logger.LogVerbose(LogType.Log, message, _root);
        }

        private void RemoveFromPool(Payload payload)
        {
            RemoveFromPool(payload.UniqueId);
        }

        public Payload FindPayload(ulong uniqueID)
        {
            return Payloads.TryGetValue(uniqueID, out Payload payload) ? payload : null;
        }
        
        private ulong GetNextFreeIndex(List<ulong> list)
        {
            list.Sort();
            IEnumerable<ulong> sequence = list;

            if (!sequence.Any()) return 1;
            if (sequence.Min() > 1) return 1;

            var found = false;

            var number = sequence.Aggregate((aggregate, next) => {
                if (found) return aggregate;

                if (next - aggregate == 1) return next;
                found = true;
                return aggregate + 1;

            });
            return found ? number : number + 1;
        }
    }
}

