using System;
using NUnit.Framework;
using OC.Data;
using OC.MaterialFlow;
using UnityEngine;
using Random = System.Random;

namespace OC.Tests.Editor.MaterialFlow
{
    public class TestDetector
    {
        private Detector _detector;
        
        [SetUp]
        public void Initialize()
        {
            var gameObject = new GameObject();
            _detector = gameObject.AddComponent<Detector>();
        }
        
        [Test]
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.None, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.None, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.None, ExpectedResult = false)]
        
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.Part, ExpectedResult = true)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.Part, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.Part, ExpectedResult = false)]
        
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.Assembly, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.Assembly, ExpectedResult = true)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.Assembly, ExpectedResult = false)]
        
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.Transport, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.Transport, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.Transport, ExpectedResult = true)]
        
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.Static, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.Static, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.Static, ExpectedResult = false)]
        
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.Storage, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.Storage, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.Storage, ExpectedResult = false)]
        
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.All, ExpectedResult = true)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.All, ExpectedResult = true)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.All, ExpectedResult = true)]
        public bool IsPayloadTypeValid(Payload.PayloadCategory entityType, CollisionFilter filter)
        {
            PayloadBase entityBase = NewPayload(entityType, 0);
            return PayloadUtils.IsTypeValid(entityBase, filter);
        }

        [Test]
        [TestCase(CollisionFilter.None, ExpectedResult = false)]
        [TestCase(CollisionFilter.Part, ExpectedResult = false)]
        [TestCase(CollisionFilter.Assembly, ExpectedResult = false)]
        [TestCase(CollisionFilter.Transport, ExpectedResult = false)]
        [TestCase(CollisionFilter.Static, ExpectedResult = true)]
        [TestCase(CollisionFilter.Storage, ExpectedResult = false)]
        [TestCase(CollisionFilter.All, ExpectedResult = true)]
        public bool IsPayloadStaticValid(CollisionFilter filter)
        {
            PayloadBase entityBase = NewPayloadStatic(0);
            return PayloadUtils.IsTypeValid(entityBase, filter);
        }

        [Test]
        [TestCase(CollisionFilter.None, ExpectedResult = false)]
        [TestCase(CollisionFilter.Part, ExpectedResult = false)]
        [TestCase(CollisionFilter.Assembly, ExpectedResult = false)]
        [TestCase(CollisionFilter.Transport, ExpectedResult = false)]
        [TestCase(CollisionFilter.Static, ExpectedResult = false)]
        [TestCase(CollisionFilter.Storage, ExpectedResult = true)]
        [TestCase(CollisionFilter.All, ExpectedResult = true)]
        public bool IsPayloadStorageValid(CollisionFilter filter)
        {
            PayloadBase entityBase = NewPayloadStorage(0);
            return PayloadUtils.IsTypeValid(entityBase, filter);
        }

        [Test]
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.None, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.None, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.None, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.Part, ExpectedResult = true)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.Part, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.Part, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.Assembly, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.Assembly, ExpectedResult = true)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.Assembly, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.Transport, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.Transport, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.Transport, ExpectedResult = true)]
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.Static, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.Static, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.Static, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.Storage, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.Storage, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.Storage, ExpectedResult = false)]
        [TestCase(Payload.PayloadCategory.Part, CollisionFilter.All, ExpectedResult = true)]
        [TestCase(Payload.PayloadCategory.Assembly, CollisionFilter.All, ExpectedResult = true)]
        [TestCase(Payload.PayloadCategory.Transport, CollisionFilter.All, ExpectedResult = true)]
        public bool AddEntityTyp(Payload.PayloadCategory entityType, CollisionFilter filter)
        {
            PayloadBase entityBase = NewPayload(entityType, 0);
            _detector.CollisionFilter = filter;
            _detector.Add(entityBase);
            return _detector.CollisionBuffer.Count > 0;
        }

        [Test]
        [TestCase(CollisionFilter.None, ExpectedResult = false)]
        [TestCase(CollisionFilter.Part, ExpectedResult = false)]
        [TestCase(CollisionFilter.Assembly, ExpectedResult = false)]
        [TestCase(CollisionFilter.Transport, ExpectedResult = false)]
        [TestCase(CollisionFilter.Static, ExpectedResult = true)]
        [TestCase(CollisionFilter.Storage, ExpectedResult = false)]
        [TestCase(CollisionFilter.All, ExpectedResult = true)]
        public bool AddEntityStatic(CollisionFilter filter)
        {
            PayloadBase entityBase = NewPayloadStatic(0);
            _detector.CollisionFilter = filter;
            _detector.Add(entityBase);
            return _detector.CollisionBuffer.Count > 0;
        }

        [Test]
        [TestCase(CollisionFilter.None, ExpectedResult = false)]
        [TestCase(CollisionFilter.Part, ExpectedResult = false)]
        [TestCase(CollisionFilter.Assembly, ExpectedResult = false)]
        [TestCase(CollisionFilter.Transport, ExpectedResult = false)]
        [TestCase(CollisionFilter.Static, ExpectedResult = false)]
        [TestCase(CollisionFilter.Storage, ExpectedResult = true)]
        [TestCase(CollisionFilter.All, ExpectedResult = true)]
        public bool AddEntityStorage(CollisionFilter filter)
        {
            PayloadBase entityBase = NewPayloadStorage(0);
            _detector.CollisionFilter = filter;
            _detector.Add(entityBase);
            return _detector.CollisionBuffer.Count > 0;
        }
        
        [Test]
        public void AddGameObject()
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<Rigidbody>();
            gameObject.AddComponent<BoxCollider>();
            gameObject.AddComponent<Payload>();
            
            _detector.CollisionFilter = CollisionFilter.All;
            _detector.Add(gameObject);
            Assert.IsTrue(_detector.Collision.Value);
        }

        [Test]
        public void AddGameObjectFailed()
        {
            _detector.Add(new GameObject());
            Assert.IsFalse(_detector.Collision.Value);
        }

        [Test]
        public void IsGroupIdValid()
        {
            var random = new Random().Next(0, 20);
            Assert.IsTrue(PayloadUtils.IsGroupValid(random,0), $"0 == {random}");
            Assert.IsTrue(PayloadUtils.IsGroupValid(random,random), $"{random} == {random}");
            Assert.IsFalse(PayloadUtils.IsGroupValid(random,random + 1), $"{random +1} == {random}");
        }
        
        [Test]
        public void Remove()
        {
            Initialize();
            const int instanceCount = 3;
            var entity = NewPayload(Payload.PayloadCategory.Part, 0);

            _detector.CollisionFilter = CollisionFilter.Part;
            _detector.GroupId = 0;
            
            _detector.Add(entity);

            for (var i = 0; i < instanceCount; i++)
            {
                _detector.Add(NewPayload(Payload.PayloadCategory.Part, 0));
            }
            
            _detector.Remove(entity);
            
            Assert.IsFalse(_detector.CollisionBuffer.Contains(entity));
        }
        
        [Test]
        public void ClearAll()
        {
            const int instanceCount = 3;
            
            _detector.CollisionFilter = CollisionFilter.Part;
            _detector.GroupId = 0;

            for (var i = 0; i < instanceCount; i++)
            {
                _detector.Add(NewPayload(Payload.PayloadCategory.Part, 0));
            }
            
            _detector.ClearAll();
            
            Assert.AreEqual(_detector.CollisionBuffer.Count, 0);
            Assert.IsFalse(_detector.Collision.Value);
        }
        
        [Test]
        public void DestroyAll()
        {
            const int instanceCount = 3;

            var entity = NewPayload(Payload.PayloadCategory.Part, 0);
            _detector.CollisionFilter = CollisionFilter.Part;
            _detector.GroupId = 0;
            
            _detector.Add(entity);
            
            for (var i = 0; i < instanceCount; i++)
            {
                _detector.Add(NewPayload(Payload.PayloadCategory.Part, 0));
            }
            
            _detector.DestroyAll();
            
            Assert.IsTrue(entity == null);
            Assert.IsFalse(_detector.Collision.Value);
        }
        
        [Test]
        public void DestroyEntityEvent()
        {
            const int instanceCount = 3;
            
            _detector.CollisionFilter = CollisionFilter.Part;
            _detector.GroupId = 0;

            for (var i = 0; i < instanceCount; i++)
            {
                _detector.Add(NewPayload(Payload.PayloadCategory.Part, 0));
            }
            
            _detector.CollisionBuffer[0].DestroyDirty();
            _detector.CollisionBuffer[1].DestroyDirty();
            
            Assert.AreEqual(1, _detector.CollisionBuffer.Count);
        }
        
        [Test]
        public void IsEmpty()
        {
            const int instanceCount = 3;
            const CollisionFilter requiredType =
                CollisionFilter.Assembly | CollisionFilter.Part | CollisionFilter.Transport | CollisionFilter.Storage;
            
            _detector.CollisionFilter = requiredType;
            _detector.GroupId = 0;
            
            foreach (Payload.PayloadCategory entityType in Enum.GetValues(typeof(Payload.PayloadCategory)))
            {
                for (var i = 0; i < instanceCount; i++)
                {
                    _detector.Add(NewPayload(entityType, 0));
                }
            }
            
            _detector.DestroyAll();
            
            Assert.IsFalse(_detector.Collision.Value);
        }

        private static Payload NewPayload(Payload.PayloadCategory type, int groupId)
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<Rigidbody>();
            gameObject.AddComponent<BoxCollider>();
            var entity = gameObject.AddComponent<Payload>();

            var discription = new PayloadDescription()
            {
                Type = (int)type,
                GroupId = groupId
            };
            entity.ApplyDescription(discription);
            return entity;
        }
        
        private static StaticCollider NewPayloadStatic(int groupId)
        {
            var gameObject = new GameObject();
            var entity = gameObject.AddComponent<StaticCollider>();
            entity.GroupId = groupId;
            return entity;
        }

        private static PayloadStorage NewPayloadStorage(int groupId)
        {
            var gameObject = new GameObject();
            var entity = gameObject.AddComponent<PayloadStorage>();
            entity.GroupId = groupId;
            return entity;
        }
    }
}


