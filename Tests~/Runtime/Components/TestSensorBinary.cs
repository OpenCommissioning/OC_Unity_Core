using System.Collections;
using NUnit.Framework;
using OC.Components;
using OC.MaterialFlow;
using UnityEngine;
using UnityEngine.TestTools;

namespace Components
{
    public class TestSensorBinary
    {
        private GameObject _testground;
        private SensorBinary[] _allSensors;
        private SensorBinary _sensorBinaryNone;
        private SensorBinary _sensorBinaryAll;
        private SensorBinary _sensorBinaryPart;
        private SensorBinary _sensorBinaryAssembly;
        private SensorBinary _sensorBinaryTransport;
        private SensorBinary _sensorBinaryStatic;
        private Payload[] _allPayloads;
        private Payload _payloadPart;
        private Payload _payloadAssembly;
        private Payload _payloadTransport;
        private StaticCollider _staticCollider;

        [UnitySetUp]
        public IEnumerator UnitySetup()
        {
            _testground = Object.Instantiate(Resources.Load<GameObject>("Prefabs/TestScene_SensorBinary"));
            _allSensors = _testground.GetComponentsInChildren<SensorBinary>();
            foreach (var sensor in _allSensors)
            {
                switch (sensor.name)
                {
                    case "SensorBinaryNone":
                        _sensorBinaryNone = sensor;
                        break;
                    case "SensorBinaryAll":
                        _sensorBinaryAll = sensor;
                        break;
                    case "SensorBinaryPart":
                        _sensorBinaryPart = sensor;
                        break;
                    case "SensorBinaryAssembly":
                        _sensorBinaryAssembly = sensor;
                        break;
                    case "SensorBinaryTransport":
                        _sensorBinaryTransport = sensor;
                        break;
                    case "SensorBinaryStatic":
                        _sensorBinaryStatic = sensor;
                        break;
                }

                _allPayloads = _testground.GetComponentsInChildren<Payload>();
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

                _staticCollider = _testground.GetComponentInChildren<StaticCollider>();
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator PartDetection()
        {
            _payloadPart.transform.position = _sensorBinaryNone.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryNone.Value.Value, "SensorBinary None detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadPart.transform.position = _sensorBinaryAll.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinaryAll.Value.Value, "SensorBinary All detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadPart.transform.position = _sensorBinaryPart.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinaryPart.Value.Value, "SensorBinary Part detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadPart.transform.position = _sensorBinaryAssembly.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryAssembly.Value.Value, "SensorBinary Assembly detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadPart.transform.position = _sensorBinaryTransport.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryTransport.Value.Value, "SensorBinary Transport detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadPart.transform.position = _sensorBinaryStatic.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryStatic.Value.Value, "SensorBinary Static detected something");
            yield return new WaitForSecondsRealtime(0.05f);
        }

        [UnityTest]
        public IEnumerator AssemblyDetection()
        {
            _payloadAssembly.transform.position = _sensorBinaryNone.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryNone.Value.Value, "SensorBinary None detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadAssembly.transform.position = _sensorBinaryAll.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinaryAll.Value.Value, "SensorBinary All detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadAssembly.transform.position = _sensorBinaryPart.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryPart.Value.Value, "SensorBinary Part detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadAssembly.transform.position = _sensorBinaryAssembly.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinaryAssembly.Value.Value, "SensorBinary Assembly detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadAssembly.transform.position = _sensorBinaryTransport.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryTransport.Value.Value, "SensorBinary Transport detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadAssembly.transform.position = _sensorBinaryStatic.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryStatic.Value.Value, "SensorBinary Static detected something");
            yield return new WaitForSecondsRealtime(0.05f);
        }

        [UnityTest]
        public IEnumerator TransportDetection()
        {
            _payloadTransport.transform.position = _sensorBinaryNone.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryNone.Value.Value, "SensorBinary None detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadTransport.transform.position = _sensorBinaryAll.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinaryAll.Value.Value, "SensorBinary All detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadTransport.transform.position = _sensorBinaryPart.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryPart.Value.Value, "SensorBinary Part detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadTransport.transform.position = _sensorBinaryAssembly.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryAssembly.Value.Value, "SensorBinary Assembly detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadTransport.transform.position = _sensorBinaryTransport.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinaryTransport.Value.Value, "SensorBinary Transport detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _payloadTransport.transform.position = _sensorBinaryStatic.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryStatic.Value.Value, "SensorBinary Static detected something");
            yield return new WaitForSecondsRealtime(0.05f);
        }

        [UnityTest]
        public IEnumerator StaticDetection()
        {
            _staticCollider.transform.position = _sensorBinaryNone.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryNone.Value.Value, "SensorBinary None detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _staticCollider.transform.position = _sensorBinaryAll.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinaryAll.Value.Value, "SensorBinary All detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);

            _staticCollider.transform.position = _sensorBinaryPart.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryPart.Value.Value, "SensorBinary Part detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _staticCollider.transform.position = _sensorBinaryAssembly.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryAssembly.Value.Value, "SensorBinary Assembly detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _staticCollider.transform.position = _sensorBinaryTransport.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsFalse(_sensorBinaryTransport.Value.Value, "SensorBinary Transport detected something");
            yield return new WaitForSecondsRealtime(0.05f);

            _staticCollider.transform.position = _sensorBinaryStatic.transform.position;
            yield return new WaitForSecondsRealtime(0.05f);
            Assert.IsTrue(_sensorBinaryStatic.Value.Value, "SensorBinary Static detected nothing");
            yield return new WaitForSecondsRealtime(0.05f);
        }

        [UnityTearDown]
        public IEnumerator Cleanup()
        {
            Object.Destroy(_testground);
            yield return null;
        }
    }
}
