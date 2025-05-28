using NUnit.Framework;
using OC.Data;
using OC.MaterialFlow;
using UnityEngine;

namespace OC.Tests.Editor.MaterialFlow
{
    public class TestEntity
    {
        private Payload _payload;

        [SetUp]
        public void Setup()
        {
            _payload = new GameObject("Entity").AddComponent<Payload>();
        }

        [Test]
        public void RequireComponentRigidbody()
        {
            var expected = _payload.TryGetComponent<Rigidbody>(out _);
            Assert.IsTrue(expected, "Rigidbody should be added automatic via RequireComponent");
        }

        [Test]
        public void RequireComponentBoxCollider()
        {
            var expected = _payload.TryGetComponent<BoxCollider>(out _);
            Assert.IsTrue(expected, "BoxCollider should be added automatic via RequireComponent");
        }

        [Test]
        [TestCase(ControlState.Ready)]
        [TestCase(ControlState.Busy)]
        [TestCase(ControlState.Done)]
        [TestCase(ControlState.Error)]
        public void SetControlState(ControlState controlState)
        {
            _payload.ControlState.Value = controlState;
            Assert.AreEqual(controlState, _payload.ControlState.Value);
        }

        [Test]
        [TestCase("Case1", Payload.PayloadCategory.Part, ControlState.Ready, PhysicState.Free)]
        [TestCase("Case2", Payload.PayloadCategory.Assembly, ControlState.Busy, PhysicState.Parent)]
        [TestCase("Case3", Payload.PayloadCategory.Transport, ControlState.Error, PhysicState.Static)]
        [TestCase("Case4", Payload.PayloadCategory.Part, ControlState.Done, PhysicState.Free)]
        [TestCase("Case5", Payload.PayloadCategory.Assembly, ControlState.Ready, PhysicState.Parent)]
        public void ApplyDiscription(string name, Payload.PayloadCategory entityType, ControlState controlState, PhysicState physicState)
        {
            var discription = new PayloadDescription()
            {
                Name = name,
                Type = (int)entityType,
                ControlState = (int)controlState,
                PhysicState = (int)physicState
            };

            _payload.ApplyDescription(discription);
            
            Assert.AreEqual(name, _payload.name, "Name is wrong");
            Assert.AreEqual(entityType, _payload.Category, "Type is wrong");
            Assert.AreEqual(controlState, _payload.ControlState.Value, "ControlState is wrong");
            Assert.AreEqual(physicState, _payload.PhysicState.Value, "PhysicState is wrong");
        }
        
        [Test]
        [TestCase(0, 0, (ulong)21)]
        [TestCase(1, 2, (ulong)31)]
        [TestCase(2, 4, (ulong)41)]
        [TestCase(3, 6, (ulong)51)]
        [TestCase(4, 8, (ulong)61)]
        [TestCase(5, 10, (ulong)71)]
        public void ApplyDiscription(int typeId, int groupId, ulong uniqueId)
        {
            var discription = new PayloadDescription()
            {
                TypeId = typeId,
                GroupId = groupId,
                UniqueId = uniqueId
            };

            _payload.ApplyDescription(discription);
            
            Assert.AreEqual(typeId, _payload.TypeId, "TypeId is wrong");
            Assert.AreEqual(groupId, _payload.GroupId, "GroupId is wrong");
            Assert.AreEqual(uniqueId, _payload.UniqueId, "UniqueId is wrong");
        }

        [Test]
        [TestCase(ControlState.Ready, ControlState.Ready, ExpectedResult = false)]
        [TestCase(ControlState.Ready, ControlState.Done, ExpectedResult = true)]
        [TestCase(ControlState.Done, ControlState.Ready, ExpectedResult = true)]
        [TestCase(ControlState.Ready, ControlState.Error, ExpectedResult = true)]
        [TestCase(ControlState.Error, ControlState.Ready, ExpectedResult = true)]
        public bool OnControlStateChanged(ControlState actualControlState, ControlState controlState)
        {
            var counter = 0;
            _payload.ControlState.Value = actualControlState;
            _payload.ControlState.OnValueChanged += _ => counter++;
            _payload.ControlState.Value = controlState;
            return counter > 0;
        }
    }
}
