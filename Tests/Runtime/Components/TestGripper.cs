using OC.MaterialFlow;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Components
{
    public class TestGripper
    {
        private GameObject _scene;
        private Gripper _gripper;
        private Gripper _gripperPick1;
        private Gripper _gripperPick2;
        
        private Pool _pool;
        private Payload _targetAssembly;
        private Payload _targetTransport;
        private const float DELAY = 0.3f;

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {
            _scene = Object.Instantiate(Resources.Load<GameObject>("Prefabs/TestScene_Gripper"));
            _pool = _scene.GetComponentInChildren<Pool>();
            _gripper = _scene.GetComponentInChildren<Gripper>();
            _gripperPick1 = GameObject.Find("Gripper_1").GetComponent<Gripper>();
            _gripperPick2 = GameObject.Find("Gripper_2").GetComponent<Gripper>();
            _targetAssembly = GameObject.Find("Assembly Target").GetComponent<Payload>();
            _targetTransport = GameObject.Find("Transport Target").GetComponent<Payload>();
            yield return null;
        }
        
        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            Object.Destroy(_scene);
            yield return null;
        }

        [UnityTest]
        [TestCase(Payload.PayloadCategory.Part, ExpectedResult = null)]
        [TestCase(Payload.PayloadCategory.Assembly, ExpectedResult = null)]
        [TestCase(Payload.PayloadCategory.Transport, ExpectedResult = null)]
        public IEnumerator PickAndPlace(Payload.PayloadCategory gripType)
        {
            yield return new WaitForSecondsRealtime(DELAY);
            _gripper.PickType = gripType;
            _gripper.Pick();
            Assert.IsTrue(_gripper.IsActive.Value);
            _gripper.Place();
            Assert.IsFalse(_gripper.IsActive.Value);
        }
        
        [UnityTest]
        public IEnumerator PickAndPlaceBusy()
        {
            yield return new WaitForSecondsRealtime(DELAY);
            _gripper.PickType = Payload.PayloadCategory.Part;
            Assert.IsTrue(!_gripper.IsActive.Value);
            Assert.IsTrue(!_gripper.IsPicked.Value);

            _gripper.Pick();
            Assert.IsTrue(_gripper.IsActive.Value);
            Assert.IsTrue(_gripper.IsPicked.Value);
            
            _gripper.Place();
            Assert.IsFalse(_gripper.IsActive.Value);
            Assert.IsFalse(_gripper.IsPicked.Value);
        }
        
        [UnityTest]
        public IEnumerator PickAndPlaceNotBusy()
        {
            yield return new WaitForSecondsRealtime(DELAY);
            _gripper.PickType = Payload.PayloadCategory.Part;
            _gripper.transform.Translate(Vector3.up);
            yield return new WaitForSecondsRealtime(0.1f);
            
            Assert.IsTrue(!_gripper.IsActive.Value);
            Assert.IsTrue(!_gripper.IsPicked.Value);

            _gripper.Pick();
            Assert.IsTrue(_gripper.IsActive.Value);
            Assert.IsFalse(_gripper.IsPicked.Value);
            
            _gripper.Place();
            Assert.IsFalse(_gripper.IsActive.Value);
            Assert.IsFalse(_gripper.IsPicked.Value);
        }

        [UnityTest]
        [TestCase(Payload.PayloadCategory.Part, ExpectedResult = null)]
        [TestCase(Payload.PayloadCategory.Assembly, ExpectedResult = null)]
        [TestCase(Payload.PayloadCategory.Transport, ExpectedResult = null)]
        public IEnumerator PickAndPlaceToPool(Payload.PayloadCategory gripType)
        {
            yield return new WaitForSecondsRealtime(DELAY);
            _gripper.PickType = gripType;
            _gripper.Pick();
            Assert.IsTrue(_gripper.IsActive.Value);
            Assert.IsTrue(_pool.transform.childCount == 0);
            
            yield return new WaitForSecondsRealtime(0.1f);
            _gripper.transform.Translate(Vector3.up);
            Assert.IsTrue(_gripper.IsActive.Value);

            yield return new WaitForSecondsRealtime(0.1f);
            _gripper.Place();
            Assert.IsTrue(!_gripper.IsActive.Value);
            Assert.IsTrue(_pool.transform.childCount > 0);
        }

        [UnityTest]
        public IEnumerator PickAndPlacePartsToAssembly()
        {
            yield return new WaitForSecondsRealtime(DELAY);
            _gripper.PickType = Payload.PayloadCategory.Part;
            _gripper.Pick();
            Assert.IsTrue(_gripper.IsActive.Value);

            _gripper.transform.Translate(Vector3.up);
            yield return new WaitForSecondsRealtime(DELAY);

            _gripper.transform.Translate(Vector3.left * 0.7f);
            yield return new WaitForSecondsRealtime(DELAY);

            _gripper.transform.Translate(Vector3.down);
            yield return new WaitForSecondsRealtime(DELAY);

            _gripper.Place();
            Assert.IsTrue(!_gripper.IsActive.Value);
            yield return new WaitForSecondsRealtime(DELAY);

            var partsInAssembly = new List<Payload>();
            foreach (Transform child in _targetAssembly.transform)
            {
                if (child.TryGetComponent<Payload>(out var part))
                {
                    partsInAssembly.Add(part);
                }
            }
            Assert.AreEqual(9, partsInAssembly.Count);
        }
        
        [UnityTest]
        public IEnumerator PickAndPlacePartsToTransport()
        {
            yield return new WaitForSecondsRealtime(DELAY);
            _gripper.PickType = Payload.PayloadCategory.Part;
            _gripper.Pick();
            Assert.IsTrue(_gripper.IsActive.Value);

            _gripper.transform.Translate(Vector3.up);
            yield return new WaitForSecondsRealtime(DELAY);

            _gripper.transform.Translate(Vector3.back);
            yield return new WaitForSecondsRealtime(DELAY);

            _gripper.transform.Translate(Vector3.down);
            yield return new WaitForSecondsRealtime(DELAY);

            _gripper.Place();
            Assert.IsTrue(!_gripper.IsActive.Value);
            yield return new WaitForSecondsRealtime(DELAY);

            var partsInTransport = new List<Payload>();
            foreach (Transform child in _targetTransport.transform)
            {
                if (child.TryGetComponent<Payload>(out var part))
                {
                    partsInTransport.Add(part);
                }
            }
            Assert.AreEqual(9, partsInTransport.Count);
        }
        
        [UnityTest]
        public IEnumerator PickAndPlaceAssemblyToTransport()
        {
            yield return new WaitForSecondsRealtime(DELAY);
            _gripper.PickType = Payload.PayloadCategory.Assembly;
            _gripper.Pick();
            Assert.IsTrue(_gripper.IsActive.Value);

            _gripper.transform.Translate(Vector3.up);
            yield return new WaitForSecondsRealtime(DELAY);

            _gripper.transform.Translate(Vector3.back);
            yield return new WaitForSecondsRealtime(DELAY);

            _gripper.transform.Translate(Vector3.down);
            yield return new WaitForSecondsRealtime(DELAY);

            _gripper.Place();
            Assert.IsTrue(!_gripper.IsActive.Value);
            yield return new WaitForSecondsRealtime(DELAY);

            var assemliesInTransport = new List<Payload>();
            foreach (Transform child in _targetTransport.transform)
            {
                if (child.TryGetComponent<Payload>(out var part))
                {
                    assemliesInTransport.Add(part);
                }
            }
            Assert.AreEqual(1, assemliesInTransport.Count);
        }

        [UnityTest]
        public IEnumerator PickFromGripper()
        {
            yield return new WaitForSecondsRealtime(DELAY);
            _gripperPick1.PickType = Payload.PayloadCategory.Part;
            _gripperPick1.Pick();
            Assert.IsTrue(_gripperPick1.IsActive.Value);
            Assert.IsTrue(_gripperPick1.IsPicked.Value);
            var count = _gripperPick1.Buffer.Count;

            _gripperPick1.transform.Translate(Vector3.up);
            yield return new WaitForSecondsRealtime(DELAY);
            
            _gripperPick2.transform.Translate(Vector3.down);
            yield return new WaitForSecondsRealtime(DELAY);
            Assert.IsTrue(_gripperPick2.Collision.Value);
            
            _gripperPick2.Pick();
            Assert.IsTrue(_gripperPick2.IsActive.Value);
            Assert.IsTrue(_gripperPick2.IsPicked.Value);
            
            Assert.AreEqual(count - _gripperPick1.Buffer.Count, _gripperPick2.Buffer.Count);
        }
    }
}
