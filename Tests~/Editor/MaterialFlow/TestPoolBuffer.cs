using System;
using System.Collections.Generic;
using NUnit.Framework;
using OC.MaterialFlow;
using System.Text.RegularExpressions;
using OC.Data;
using UnityEngine;
using UnityEngine.TestTools;

namespace OC.Tests.Editor.MaterialFlow
{
    public class TestPoolManager
    {
        private PoolManager _poolManager;

        private static Payload CreatePayload(int typeId = 0, uint uniqueId = 0, GameObject parent = null)
        {
            var gameObject = new GameObject();
            gameObject.AddComponent<Rigidbody>();
            gameObject.AddComponent<BoxCollider>();
            var entity = gameObject.AddComponent<Payload>();

            var description = new PayloadDescription()
            {
                TypeId = typeId,
                UniqueId = uniqueId
            };

            entity.ApplyDescription(description);

            if (parent != null) gameObject.transform.parent = parent.transform;
            return entity;
        }
        
        [SetUp]
        public void SetUp()
        {
            _poolManager = new PoolManager();
            _poolManager.PayloadList.Add(CreatePayload());
            _poolManager.PayloadList.Add(CreatePayload(1));
            _poolManager.PayloadList.Add(CreatePayload(2));
        }

        [Test]
        [TestCase(-1, false)]
        [TestCase(0, true)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        [TestCase(3, false)]
        public void IsTypeValid(int typeId,bool expected)
        {
            Assert.That(expected == _poolManager.IsTypeValid(typeId), $"Pool Prefabs Count: {_poolManager.Payloads.Count}, Input TypeId {typeId}");
        }
        
        [Test]
        [TestCase(-1, 0)]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 2)]
        public void GetValidType(int typeId, int expectedTypeId)
        {
            Assert.That(expectedTypeId == _poolManager.GetValidType(typeId), $"Pool Prefabs Count: {_poolManager.Payloads.Count}, Input FolderId {typeId}");
        }
        
        [Test]
        public void InvalidTypeIdLog()
        {
            var payload = CreatePayload(5);
            _poolManager.Registrate(payload);
            LogAssert.Expect(LogType.Error, new Regex(@"(isn't valid)"));
        }

        [Test]
        public void InvalidUniqueIdLog()
        {
            var payload1 = CreatePayload(0, 1);
            var payload2= CreatePayload(0, 1);
            _poolManager.Registrate(payload1);
            _poolManager.Registrate(payload2);
            LogAssert.Expect(LogType.Error, new Regex(@"(already exists)"));
        }
        
        [Test]
        [TestCase(-1, -1)]
        [TestCase(2, -1)]
        public void GetValidTypeNullPrefabs(int typeId, int expectedTypeId)
        {
            var poolManager = new PoolManager();
            Assert.That(expectedTypeId == poolManager.GetValidType(typeId), $"Pool Prefabs Count: {_poolManager.Payloads.Count}, Input FolderId {typeId}");
        }
        
        [Test]
        [TestCase((ulong)0, 0, ExpectedResult = 1)]
        [TestCase((ulong)1, 0, ExpectedResult = 1)]
        [TestCase((ulong)2, 1,  ExpectedResult = 2)]
        [TestCase((ulong)3, 2,  ExpectedResult = 3)]
        [TestCase((ulong)6, 5,  ExpectedResult = 6)]
        public int Instantiate(ulong uniqueId, int actualCount)
        {
            for (var i = 1; i <= actualCount; i++)
            {
                _poolManager.Instantiate(Vector3.zero, Quaternion.identity, 0, (ulong)i);
            }
            
            _poolManager.Instantiate(Vector3.zero, Quaternion.identity, 0, uniqueId);
            return _poolManager.Count;
        }

        [Test]
        [TestCaseSource(nameof(GetEntityInfos))]
        public void InstantiateWithInfo(PayloadDescriptionTestCase payloadDescriptionTestCase)
        {
            for (var i = 1; i <= payloadDescriptionTestCase.ActualCount; i++)
            {
                _poolManager.Instantiate(Vector3.zero, Quaternion.identity, 0, (ulong)i);
            }

            LogAssert.ignoreFailingMessages = true;
            _poolManager.Instantiate(payloadDescriptionTestCase.Description);
            Assert.AreEqual(payloadDescriptionTestCase.ExpectedCount, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {payloadDescriptionTestCase.ExpectedCount}");
        }

        public static IEnumerable<PayloadDescriptionTestCase> GetEntityInfos()
        {
            yield return new PayloadDescriptionTestCase
            {
                Description = new PayloadDescription(){Type = 1, UniqueId = 1, Transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one)}, 
                ActualCount = 3, 
                ExpectedCount = 3,
            };
            yield return new PayloadDescriptionTestCase
            {
                Description = new PayloadDescription{Type = 1, UniqueId = 4, Transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one)}, 
                ActualCount = 3, 
                ExpectedCount = 4,
            };
            yield return new PayloadDescriptionTestCase
            {
                Description = new PayloadDescription{Type = 1, UniqueId = 5, Transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one)}, 
                ActualCount = 5, 
                ExpectedCount = 5,
            };
            yield return new PayloadDescriptionTestCase
            {
                Description = new PayloadDescription{Type = 1, UniqueId = 6, Transform = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one)}, 
                ActualCount = 5, 
                ExpectedCount = 6,
            };
        }

        public struct PayloadDescriptionTestCase
        {
            public PayloadDescription Description;
            public int ActualCount;
            public int ExpectedCount;
        }
        
        [Test]
        [TestCase((ulong)1, 1, 1)]
        [TestCase((ulong)1, 2, 2)]
        [TestCase((ulong)1, 3, 3)]
        [TestCase((ulong)3, 5, 5)]
        public void InstantiateWithWrongId(ulong uniqueId, int actualId, int expectedCounter)
        {
            for (var i = 1; i <= actualId; i++)
            {
                _poolManager.Instantiate(Vector3.zero, Quaternion.identity, 0, (ulong)i);
            }
            
            _poolManager.Instantiate(Vector3.zero, Quaternion.identity, 0, uniqueId);
            LogAssert.Expect(LogType.Error, new Regex(@"(already exists)"));
            Assert.AreEqual(expectedCounter, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {expectedCounter}");
        }

        [Test]
        public void RegisterParts()
        {
            const int expected = 3;
            for (var i = 0; i < expected; i++)
            {
                _poolManager.Registrate(CreatePayload());
            }
            Assert.AreEqual(expected, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {expected}");

            _poolManager.Registrate(CreatePayload(uniqueId: 1));
            LogAssert.Expect(LogType.Error, new Regex(@"(already exists)"));
            Assert.AreEqual(expected, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {expected}");
            
            _poolManager.Registrate(CreatePayload(typeId: 4));
            LogAssert.Expect(LogType.Error, new Regex(@"(isn't valid)"));
            Assert.AreEqual(expected, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {expected}");
        }
        
        [Test]
        public void UnregistrParts()
        {
            const int partsCount = 4;
            var parts = new List<Payload>();

            for (var i = 0; i < partsCount; i++)
            {
                var part = CreatePayload();
                parts.Add(part);
                _poolManager.Registrate(part);
            }

            for (var i = 0; i < partsCount; i++)
            {
                var expectedCount = partsCount - (i + 1);
                _poolManager.Unregistrate(parts[i]);
                Assert.AreEqual(expectedCount, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {expectedCount}");
            }
        }
        
        [Test]
        public void Registrssembly()
        {
            var assembly = CreatePayload();
            CreatePayload(parent: assembly.gameObject);
            CreatePayload(parent: assembly.gameObject);
            CreatePayload(parent: assembly.gameObject);
            
            _poolManager.Registrate(assembly);
            Assert.AreEqual(4, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {4}");
        }
        
        [Test]
        public void UnregistrAssembly()
        {
            var assembly = CreatePayload();
            var partA = CreatePayload(parent: assembly.gameObject);
            CreatePayload(parent: assembly.gameObject);
            CreatePayload(parent: assembly.gameObject);
            
            _poolManager.Registrate(assembly);

            _poolManager.Unregistrate(partA);
            Assert.AreEqual(3, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {3}");
            _poolManager.Unregistrate(assembly);
            Assert.AreEqual(0, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {0}");
        }
        
        [Test]
        public void UnregistrInvalidPart()
        {
            var partA = CreatePayload();
            var partB = CreatePayload();
            partB.Registrate(123);

            _poolManager.Registrate(partA);
            
            Assert.Throws<Exception>(()=> _poolManager.Unregistrate(partB));
            LogAssert.Expect(LogType.Error, new Regex(@"(isn't contains in pool)"));
            Assert.AreEqual(1, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {1}");
        }
        
        [Test]
        public void UnregistrInvalidUniqueId()
        {
            var partA = CreatePayload(uniqueId: 1);
            var partB = CreatePayload(uniqueId: 2);

            _poolManager.Registrate(partA);
            _poolManager.Registrate(partB);
            partB.Registrate(123);
            
            Assert.Throws<Exception>(()=> _poolManager.Unregistrate(partB));
            LogAssert.Expect(LogType.Error, new Regex(@"(isn't contains in pool)"));
            Assert.AreEqual(2, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {1}");
        }
        
        [Test]
        public void DestroyParts()
        {
            const int partsCount = 4;
            var parts = new List<Payload>();

            for (var i = 0; i < partsCount; i++)
            {
                var part = CreatePayload();
                parts.Add(part);
                _poolManager.Registrate(part);
            }

            for (var i = 0; i < partsCount; i++)
            {
                var expectedCount = partsCount - (i + 1);
                _poolManager.Destroy(parts[i]);
                Assert.AreEqual(expectedCount, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {expectedCount}");
            }
        }
        
        [Test]
        public void DestroyAssembly()
        {
            var part = CreatePayload();
            var assembly = CreatePayload();
            CreatePayload(parent: assembly.gameObject);
            CreatePayload(parent: assembly.gameObject);
            CreatePayload(parent: assembly.gameObject);
            
            _poolManager.Registrate(part);
            _poolManager.Registrate(assembly);

            _poolManager.Destroy(assembly);

            Assert.AreEqual(1, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {1}");
        }
        
        [Test]
        public void DestroyAll()
        {
            var part = CreatePayload();
            var assembly = CreatePayload();
            CreatePayload(parent: assembly.gameObject);
            CreatePayload(parent: assembly.gameObject);
            CreatePayload(parent: assembly.gameObject);

            _poolManager.Registrate(assembly);
            _poolManager.Registrate(part);

            _poolManager.DestroyAll();
            Assert.AreEqual(0, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {0}");
        }

        [Test]
        public void ReplaceLogs()
        {
            var source = new GameObject("TestSource");
            var part = CreatePayload(typeId: 1);
            _poolManager.Registrate(part);

            _poolManager.Replace(source, null,0);
            LogAssert.Expect(LogType.Error, new Regex(@"(Payload is null)"));
            
            _poolManager.Replace(source, part,1);
            LogAssert.Expect(LogType.Error, new Regex(@"(is equal)"));
            
            _poolManager.Replace(source, part,4);
            LogAssert.Expect(LogType.Error, new Regex(@"(as target isn't valid)"));
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(9)]
        public void ReplacePart(int uniqueId)
        {
            const int entitiesCount = 10;
            var source = new GameObject("TestSource");
            for (var i = 1; i <= entitiesCount; i++)
            {
                _poolManager.Registrate(CreatePayload(1, (uint)i));
            }

            var part = _poolManager.Payloads[(ulong)uniqueId];
            _poolManager.Replace(source, part, 2);
            Assert.AreEqual(entitiesCount, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {entitiesCount}");

            var replacedPart = _poolManager.Payloads[(ulong)uniqueId];
            Assert.AreEqual(2, replacedPart.TypeId, "TypeId is not correct");
            Assert.AreEqual(uniqueId, replacedPart.UniqueId, "UniqueId is not correct");

            foreach (var item in _poolManager.Payloads.Values)
            {
                Assert.IsNotNull(item);
            }
        }

        [Test]
        [TestCase(1, 2, ExpectedResult = false)]
        [TestCase(1, 0, ExpectedResult = false)]
        [TestCase(5, 0, ExpectedResult = false)]
        [TestCase(5, 4, ExpectedResult = false)]
        [TestCase(1, 6, ExpectedResult = true)]
        [TestCase(5, 9, ExpectedResult = true)]
        [Obsolete("Obsolete")]
        public bool RegistrWithNewUniqueId(int actualUniqueId, int newUniqueId)
        {
            const int entitiesCount = 5;
            for (var i = 1; i <= entitiesCount; i++)
            {
                _poolManager.Registrate(CreatePayload(1, (uint)i));
            }

            var part = _poolManager.Payloads[(ulong)actualUniqueId];

            Debug.logger.logEnabled = false;
            var result = _poolManager.ChangeRegistration(part, (ulong)newUniqueId).Item1;
            Debug.logger.logEnabled = true;
            return result;
        }
        
        [Test]
        [TestCase(0, true)]
        [TestCase(1, false)]
        [TestCase(5, false)]
        [TestCase(6, true)]
        [TestCase(7, true)]
        public void IsUniqueIdValid(int uniqueId, bool expected)
        {
            const int entitiesCount = 5;
            for (var i = 1; i <= entitiesCount; i++)
            {
                _poolManager.Registrate(CreatePayload(1, (uint)i));
            }

            var result = _poolManager.IsUniqueIdValid((ulong)uniqueId);
            Assert.AreEqual(expected, result, "UniqueId is wrong");
        }

        [Test]
        [TestCase(0, ExpectedResult = 0)]
        [TestCase(1, ExpectedResult = 6)]
        [TestCase(5, ExpectedResult = 6)]
        [TestCase(6, ExpectedResult = 6)]
        [TestCase(7, ExpectedResult = 7)]
        public int GetPossibleUniqueId(int uniqueId)
        {
            const int entitiesCount = 5;
            for (var i = 1; i <= entitiesCount; i++)
            {
                _poolManager.Registrate(CreatePayload(1, (uint)i));
            }

            return (int)_poolManager.GetPossibleUniqueId((ulong)uniqueId);
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void Repair(int uniqueId)
        {
            const int entitiesCount = 5;
            for (var i = 1; i <= entitiesCount; i++)
            {
                _poolManager.Registrate(CreatePayload(1, (uint)i));
            }

            _poolManager.Payloads[(ulong)uniqueId] = null;
            _poolManager.Repair();
            LogAssert.Expect(LogType.Warning, new Regex(@$"UniqueId {uniqueId} is not valid"));
            Assert.AreEqual(entitiesCount-1, _poolManager.Count, $"Pool Entities Count: {_poolManager.Count}, Expected: {entitiesCount-1}");
        }
    }
}
