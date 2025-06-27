using System.Collections;
using NUnit.Framework;
using OC.Components;
using OC.MaterialFlow;
using UnityEngine;
using UnityEngine.TestTools;

namespace OC.Tests.Runtime.Components
{
    public class TestSensorBinaryScene
    {
        private GameObject _testGround;
        private SensorBinary[] _allSensors;
        private SensorBinary _sensorBinarySceneNone;
        private SensorBinary _sensorBinarySceneAll;
        private SensorBinary _sensorBinaryScenePart;
        private SensorBinary _sensorBinarySceneAssembly;
        private SensorBinary _sensorBinarySceneTransport;
        private SensorBinary _sensorBinarySceneStatic;
        private Payload[] _allPayloads;
        private Payload _payloadPart;
        private Payload _payloadAssembly;
        private Payload _payloadTransport;
        private StaticCollider _staticCollider;

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {
            _testGround = Object.Instantiate(Resources.Load<GameObject>("Prefabs/TestScene_SensorBinary"));
            _allSensors = _testGround.GetComponentsInChildren<SensorBinary>();
            foreach (var sensor in _allSensors)
            {
                switch (sensor.name)
                {
                    case "SensorBinaryNone":
                        _sensorBinarySceneNone = sensor;
                        break;
                    case "SensorBinaryAll":
                        _sensorBinarySceneAll = sensor;
                        break;
                    case "SensorBinaryPart":
                        _sensorBinaryScenePart = sensor;
                        break;
                    case "SensorBinaryAssembly":
                        _sensorBinarySceneAssembly = sensor;
                        break;
                    case "SensorBinaryTransport":
                        _sensorBinarySceneTransport = sensor;
                        break;
                    case "SensorBinaryStatic":
                        _sensorBinarySceneStatic = sensor;
                        break;
                }

                _allPayloads = _testGround.GetComponentsInChildren<Payload>();
                foreach (var payload in _allPayloads)
                {
                    switch (payload.name)
                    {
                        case "PayloadPart":
                            _payloadPart = payload;
                            break;
                        case "PayloadAssembly":
                            _payloadAssembly = payload;
                            break;
                        case "PayloadTransport":
                            _payloadTransport = payload;
                            break;
                    }
                }

                _staticCollider = _testGround.GetComponentInChildren<StaticCollider>();
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator PartDetection()
        {
            _payloadPart.transform.position = _sensorBinarySceneNone.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneNone.Value.Value, "SensorBinary None detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadPart.transform.position = _sensorBinarySceneAll.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinarySceneAll.Value.Value, "SensorBinary All detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadPart.transform.position = _sensorBinaryScenePart.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinaryScenePart.Value.Value, "SensorBinary Part detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadPart.transform.position = _sensorBinarySceneAssembly.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneAssembly.Value.Value, "SensorBinary Assembly detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadPart.transform.position = _sensorBinarySceneTransport.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneTransport.Value.Value, "SensorBinary Transport detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadPart.transform.position = _sensorBinarySceneStatic.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneStatic.Value.Value, "SensorBinary Static detected something");
            yield return new WaitForSecondsRealtime(0.05f);
        }

        [UnityTest]
        public IEnumerator AssemblyDetection()
        {
            _payloadAssembly.transform.position = _sensorBinarySceneNone.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneNone.Value.Value, "SensorBinary None detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadAssembly.transform.position = _sensorBinarySceneAll.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinarySceneAll.Value.Value, "SensorBinary All detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadAssembly.transform.position = _sensorBinaryScenePart.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryScenePart.Value.Value, "SensorBinary Part detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadAssembly.transform.position = _sensorBinarySceneAssembly.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinarySceneAssembly.Value.Value, "SensorBinary Assembly detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadAssembly.transform.position = _sensorBinarySceneTransport.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneTransport.Value.Value, "SensorBinary Transport detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadAssembly.transform.position = _sensorBinarySceneStatic.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneStatic.Value.Value, "SensorBinary Static detected something");
            yield return new WaitForSecondsRealtime(0.05f);
        }

        [UnityTest]
        public IEnumerator TransportDetection()
        {
            _payloadTransport.transform.position = _sensorBinarySceneNone.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneNone.Value.Value, "SensorBinary None detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadTransport.transform.position = _sensorBinarySceneAll.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinarySceneAll.Value.Value, "SensorBinary All detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadTransport.transform.position = _sensorBinaryScenePart.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryScenePart.Value.Value, "SensorBinary Part detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadTransport.transform.position = _sensorBinarySceneAssembly.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneAssembly.Value.Value, "SensorBinary Assembly detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadTransport.transform.position = _sensorBinarySceneTransport.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinarySceneTransport.Value.Value, "SensorBinary Transport detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadTransport.transform.position = _sensorBinarySceneStatic.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneStatic.Value.Value, "SensorBinary Static detected something");
            yield return new WaitForSecondsRealtime(0.05f);
        }

        [UnityTest]
        public IEnumerator StaticDetection()
        {
            _staticCollider.transform.position = _sensorBinarySceneNone.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneNone.Value.Value, "SensorBinary None detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _staticCollider.transform.position = _sensorBinarySceneAll.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinarySceneAll.Value.Value, "SensorBinary All detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _staticCollider.transform.position = _sensorBinaryScenePart.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryScenePart.Value.Value, "SensorBinary Part detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _staticCollider.transform.position = _sensorBinarySceneAssembly.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneAssembly.Value.Value, "SensorBinary Assembly detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _staticCollider.transform.position = _sensorBinarySceneTransport.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinarySceneTransport.Value.Value, "SensorBinary Transport detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _staticCollider.transform.position = _sensorBinarySceneStatic.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinarySceneStatic.Value.Value, "SensorBinary Static detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);
        }

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            Object.Destroy(_testGround);
            yield return null;
        }
    }
}
