using System;
using NUnit.Framework;
using OC.Data;
using OC.MaterialFlow;
using UnityEngine;
using Random = System.Random;

namespace OC.Tests.Editor.MaterialFlow
{
    public class TestCollisionDetector
    {
        private CollisionDetector _collisionDetector;
        
        [SetUp]
        public void Initilize()
        {
            _collisionDetector = new CollisionDetector();
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
        public bool IsEntityTypeValid(Payload.PayloadCategory entityType, CollisionFilter filter)
        {
            PayloadBase entityBase = NewEntity(entityType, 0);
            return CollisionDetector.IsTypeValid(entityBase, filter);
        }

        [Test]
        [TestCase(CollisionFilter.None, ExpectedResult = false)]
        [TestCase(CollisionFilter.Part, ExpectedResult = false)]
        [TestCase(CollisionFilter.Assembly, ExpectedResult = false)]
        [TestCase(CollisionFilter.Transport, ExpectedResult = false)]
        [TestCase(CollisionFilter.Static, ExpectedResult = true)]
        [TestCase(CollisionFilter.Storage, ExpectedResult = false)]
        [TestCase(CollisionFilter.All, ExpectedResult = true)]
        public bool IsEntityStaticValid(CollisionFilter filter)
        {
            PayloadBase entityBase = NewEntityStatic(0);
            return CollisionDetector.IsTypeValid(entityBase, filter);
        }

        [Test]
        [TestCase(CollisionFilter.None, ExpectedResult = false)]
        [TestCase(CollisionFilter.Part, ExpectedResult = false)]
        [TestCase(CollisionFilter.Assembly, ExpectedResult = false)]
        [TestCase(CollisionFilter.Transport, ExpectedResult = false)]
        [TestCase(CollisionFilter.Static, ExpectedResult = false)]
        [TestCase(CollisionFilter.Storage, ExpectedResult = true)]
        [TestCase(CollisionFilter.All, ExpectedResult = true)]
        public bool IsEntityStorgeValid(CollisionFilter filter)
        {
            PayloadBase entityBase = NewEntityStorage(0);
            return CollisionDetector.IsTypeValid(entityBase, filter);
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
            PayloadBase entityBase = NewEntity(entityType, 0);
            _collisionDetector.Filter = filter;
            _collisionDetector.Add(entityBase);
            return _collisionDetector.Buffer.Count > 0;
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
            PayloadBase entityBase = NewEntityStatic(0);
            _collisionDetector.Filter = filter;
            return _collisionDetector.Add(entityBase);
        }

        [Test]
        [TestCase(CollisionFilter.None, ExpectedResult = false)]
        [TestCase(CollisionFilter.Part, ExpectedResult = false)]
        [TestCase(CollisionFilter.Assembly, ExpectedResult = false)]
        [TestCase(CollisionFilter.Transport, ExpectedResult = false)]
        [TestCase(CollisionFilter.Static, ExpectedResult = false)]
        [TestCase(CollisionFilter.Storage, ExpectedResult = true)]
        [TestCase(CollisionFilter.All, ExpectedResult = true)]
        public bool AddEntitityStorage(CollisionFilter filter)
        {
            PayloadBase entityBase = NewEntityStorage(0);
            _collisionDetector.Filter = filter;
            return _collisionDetector.Add(entityBase);
        }
        
        [Test]
        public void AddGameObject()
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<Rigidbody>();
            gameObject.AddComponent<BoxCollider>();
            gameObject.AddComponent<Payload>();
            
            _collisionDetector.Filter = CollisionFilter.All;
            _collisionDetector.Add(gameObject);
            Assert.IsTrue(_collisionDetector.Collision);
        }

        [Test]
        public void AddGameObjectFailed()
        {
            _collisionDetector.Add(new GameObject());
            Assert.IsFalse(_collisionDetector.Collision);
        }

        [Test]
        public void IsGroupIdValid()
        {
            var random = new Random().Next(0, 20);
            Assert.IsTrue(CollisionDetector.IsGroupValid(random,0), $"0 == {random}");
            Assert.IsTrue(CollisionDetector.IsGroupValid(random,random), $"{random} == {random}");
            Assert.IsFalse(CollisionDetector.IsGroupValid(random,random + 1), $"{random +1} == {random}");
        }
        
        [Test]
        public void Remove()
        {
            Initilize();
            const int instanceCount = 3;
            var entity = NewEntity(Payload.PayloadCategory.Part, 0);

            _collisionDetector.Filter = CollisionFilter.Part;
            _collisionDetector.GroupId = 0;
            
            _collisionDetector.Add(entity);

            for (var i = 0; i < instanceCount; i++)
            {
                _collisionDetector.Add(NewEntity(Payload.PayloadCategory.Part, 0));
            }
            
            _collisionDetector.Remove(entity);
            
            Assert.IsFalse(_collisionDetector.Buffer.Contains(entity));
        }
        
        [Test]
        public void ClearAll()
        {
            const int instanceCount = 3;
            
            _collisionDetector.Filter = CollisionFilter.Part;
            _collisionDetector.GroupId = 0;

            for (var i = 0; i < instanceCount; i++)
            {
                _collisionDetector.Add(NewEntity(Payload.PayloadCategory.Part, 0));
            }
            
            _collisionDetector.ClearAll();
            
            Assert.AreEqual(_collisionDetector.Buffer.Count, 0);
            Assert.IsFalse(_collisionDetector.Collision.Value);
        }
        
        [Test]
        public void DestroyAll()
        {
            const int instanceCount = 3;

            var entity = NewEntity(Payload.PayloadCategory.Part, 0);
            _collisionDetector.Filter = CollisionFilter.Part;
            _collisionDetector.GroupId = 0;
            
            _collisionDetector.Add(entity);
            
            for (var i = 0; i < instanceCount; i++)
            {
                _collisionDetector.Add(NewEntity(Payload.PayloadCategory.Part, 0));
            }
            
            _collisionDetector.DestroyAll();
            
            Assert.IsTrue(entity == null);
            Assert.IsFalse(_collisionDetector.Collision);
        }
        
        [Test]
        public void DestroyEntityEvent()
        {
            const int instanceCount = 3;
            
            _collisionDetector.Filter = CollisionFilter.Part;
            _collisionDetector.GroupId = 0;

            for (var i = 0; i < instanceCount; i++)
            {
                _collisionDetector.Add(NewEntity(Payload.PayloadCategory.Part, 0));
            }
            
            _collisionDetector.Buffer[0].DestroyDirty();
            _collisionDetector.Buffer[1].DestroyDirty();
            
            Assert.AreEqual(1, _collisionDetector.Buffer.Count);
        }
        
        [Test]
        public void IsEmpty()
        {
            const int instanceCount = 3;
            const CollisionFilter requiredType =
                CollisionFilter.Assembly | CollisionFilter.Part | CollisionFilter.Transport | CollisionFilter.Storage;
            
            _collisionDetector.Filter = requiredType;
            _collisionDetector.GroupId = 0;
            
            foreach (Payload.PayloadCategory entityType in Enum.GetValues(typeof(Payload.PayloadCategory)))
            {
                for (var i = 0; i < instanceCount; i++)
                {
                    _collisionDetector.Add(NewEntity(entityType, 0));
                }
            }
            
            _collisionDetector.DestroyAll();
            
            Assert.IsFalse(_collisionDetector.Collision);
        }

        private static Payload NewEntity(Payload.PayloadCategory type, int groupId)
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
        
        private static StaticCollider NewEntityStatic(int groupId)
        {
            var gameObject = new GameObject();
            var entity = gameObject.AddComponent<StaticCollider>();
            entity.GroupId = groupId;
            return entity;
        }

        private static PayloadStorage NewEntityStorage(int groupId)
        {
            var gameObject = new GameObject();
            var entity = gameObject.AddComponent<PayloadStorage>();
            entity.GroupId = groupId;
            return entity;
        }
    }
}


