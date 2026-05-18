using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using OC;
using UnityEngine.TestTools.Utils;
using OC.Components;

namespace OC.Tests.Runtime.Components
{
    public class TestAxis
    {
        private Axis _axis;
        private Axis _slaveAxis;
        private readonly Vector3 _initPosition = new (1f, 1f, -1f);
        private readonly Quaternion _initRotation = Quaternion.Euler(10f, 10f, -10f);
        private readonly Vector3 _initPositionSlave = new (2f, 2f, -2f);
        private readonly Quaternion _initRotationSlave = Quaternion.Euler(40f, 40f, -40f);
        private const float SLAVE_FACTOR = 2.4f;
        private const float DELAY = 0.3f;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            _axis = new GameObject().AddComponent<Axis>();
            _axis.transform.SetPositionAndRotation(_initPosition, _initRotation);
            _slaveAxis = new GameObject().AddComponent<Axis>();
            _slaveAxis.transform.SetPositionAndRotation(_initPositionSlave, _initRotationSlave);
            _slaveAxis.Actor = _axis;
            _slaveAxis.Factor = SLAVE_FACTOR;
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            Object.Destroy(_axis.gameObject);
            Object.Destroy(_slaveAxis.gameObject);
            yield return null;
        }

        public static IEnumerable<AxisTestCase> GetTestCasesPosition()
        {
            yield return new AxisTestCase(AxisDirection.X, 0, 1);
            yield return new AxisTestCase(AxisDirection.X, 1, 1);
            yield return new AxisTestCase(AxisDirection.X, -1, 2);
            yield return new AxisTestCase(AxisDirection.Y, 0, 1);
            yield return new AxisTestCase(AxisDirection.Y, 1, 1);
            yield return new AxisTestCase(AxisDirection.Y, -1, 2);
            yield return new AxisTestCase(AxisDirection.Z, 0, 1);
            yield return new AxisTestCase(AxisDirection.Z, 1, 1);
            yield return new AxisTestCase(AxisDirection.Z, -1, 2);
        }
        
        public static IEnumerable<AxisTestCase> GetTestCasesRotation()
        {
            yield return new AxisTestCase(AxisDirection.X, 0, 1);
            yield return new AxisTestCase(AxisDirection.X, 25, 1);
            yield return new AxisTestCase(AxisDirection.X, -25, 2);
            yield return new AxisTestCase(AxisDirection.Y, 0, 1);
            yield return new AxisTestCase(AxisDirection.Y, 25, 1);
            yield return new AxisTestCase(AxisDirection.Y, -25, 2);
            yield return new AxisTestCase(AxisDirection.Z, 0, 1);
            yield return new AxisTestCase(AxisDirection.Z, 25, 1);
            yield return new AxisTestCase(AxisDirection.Z, -25, 2);
        }

        public class AxisTestCase
        {
            public readonly AxisDirection Direction;
            public readonly float Target;
            public readonly float Factor;

            public AxisTestCase(AxisDirection direction, float target, float factor)
            {
                Direction = direction;
                Target = target;
                Factor = factor;
            }
        }

        [UnityTest]
        public IEnumerator MoveTo([ValueSource(nameof(GetTestCasesPosition))] AxisTestCase testCase)
        {
            _axis.Type = AxisType.Translation;
            _axis.Direction = testCase.Direction;
            _axis.Factor = testCase.Factor;
            _axis.Target.Value = testCase.Target;
            
            yield return new WaitForSecondsRealtime(DELAY);

            var expected = _initPosition + _initRotation * Math.GetDirection(testCase.Direction) * testCase.Target * testCase.Factor;
            Assert.That(_axis.transform.localPosition, Is.EqualTo(expected).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!");
            Assert.That(_axis.transform.localRotation, Is.EqualTo(_initRotation).Using(QuaternionEqualityComparer.Instance), "Local rotation isn´t correct!");
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator RotateTo([ValueSource(nameof(GetTestCasesRotation))] AxisTestCase testCase)
        {
            _axis.Type = AxisType.Rotation;
            _axis.Direction = testCase.Direction;
            _axis.Factor = testCase.Factor;
            _axis.Target.Value = testCase.Target;
            
            yield return new WaitForSecondsRealtime(DELAY);

            var expected = _initRotation * Quaternion.Euler(Math.GetDirection(testCase.Direction) * testCase.Target * testCase.Factor);
            Assert.That(_axis.transform.localPosition, Is.EqualTo(_initPosition).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!");
            Assert.That(_axis.transform.localRotation, Is.EqualTo(expected).Using(QuaternionEqualityComparer.Instance), "Local rotation isn´t correct!");
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator MoveToWithSlave([ValueSource(nameof(GetTestCasesPosition))] AxisTestCase testCase)
        {
            _axis.Type = AxisType.Translation;
            _axis.Direction = testCase.Direction;
            _axis.Factor = testCase.Factor;
            
            _slaveAxis.Type = AxisType.Translation;
            _slaveAxis.Direction = testCase.Direction;
            
            _axis.Target.Value = testCase.Target;
            
            yield return new WaitForSecondsRealtime(DELAY);

            var expected = _initPositionSlave + _initRotationSlave * Math.GetDirection(testCase.Direction) * testCase.Target * testCase.Factor * SLAVE_FACTOR;
            Assert.That(_slaveAxis.transform.localPosition, Is.EqualTo(expected).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!");
            Assert.That(_slaveAxis.transform.localRotation, Is.EqualTo(_initRotationSlave).Using(QuaternionEqualityComparer.Instance), "Local rotation isn´t correct!");
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator RotateToWithSlave([ValueSource(nameof(GetTestCasesRotation))] AxisTestCase testCase)
        {
            _axis.Type = AxisType.Rotation;
            _axis.Direction = testCase.Direction;
            _axis.Factor = testCase.Factor;
            
            _slaveAxis.Type = AxisType.Rotation;
            _slaveAxis.Direction = testCase.Direction;
            
            _axis.Target.Value = testCase.Target;
            
            yield return new WaitForSecondsRealtime(DELAY);

            var expected = _initRotationSlave * Quaternion.Euler(Math.GetDirection(testCase.Direction) * testCase.Target * testCase.Factor * SLAVE_FACTOR);
            Assert.That(_slaveAxis.transform.localPosition, Is.EqualTo(_initPositionSlave).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!");
            Assert.That(_slaveAxis.transform.localRotation, Is.EqualTo(expected).Using(QuaternionEqualityComparer.Instance), "Local rotation isn´t correct!");
            yield return null;
        }
    }
}

