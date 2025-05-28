using OC.Components;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using OC.Tests;
using Assert = UnityEngine.Assertions.Assert;

namespace Components
{
    public class TestCylinder
    {
        private Cylinder _cylinder;

        [UnitySetUp]
        public IEnumerator UnitySetUp()
        {
            _cylinder = new GameObject().AddComponent<Cylinder>();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator UnityTearDown()
        {
            Object.Destroy(_cylinder.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator MoveToLimit([ValueSource(nameof(GetMoveToLimitCase))]MoveToLimitCase testCase)
        {
            _cylinder.TimeToMin.Value = 0.01f;
            _cylinder.TimeToMax.Value = 0.01f;
            
            _cylinder.Type.Value = testCase.Type;
            _cylinder.JogMinus = testCase.JogMinus;
            _cylinder.JogPlus = testCase.JogPlus;
            
            yield return new WaitForSeconds(0.5f);
            
            Assert.AreApproximatelyEqual(testCase.ExpectedValue, _cylinder.Value.Value, Utils.TEST_TOLERANCE, "Value isn't correct");
            Assert.AreEqual(testCase.ExpectedLimitMin, _cylinder.OnLimitMin.Value, "OnLimitMin isn't correct");
            Assert.AreEqual(testCase.ExpectedLimitMax, _cylinder.OnLimitMax.Value, "OnLimitMax isn't correct");
            yield return null;
        }

        [UnityTest]
        public IEnumerator MoveToTime([ValueSource(nameof(GetMoveTimeCase))] float timeTo)
        {
            _cylinder.TimeToMin.Value = timeTo;
            _cylinder.TimeToMax.Value = timeTo;
            _cylinder.Limits.Value = new Vector2(0, 10);
            
            _cylinder.JogMinus = false;
            _cylinder.JogPlus = true;
            var startTime = Time.time;
            yield return new WaitUntil(() => _cylinder.OnLimitMax.Value);
            var duration = Time.time - startTime;
            Assert.AreApproximatelyEqual(timeTo, duration, Utils.TEST_TOLERANCE, "Duration in plus direction isn't correct");
            
            _cylinder.JogMinus = true;
            _cylinder.JogPlus = false;
            startTime = Time.time;
            yield return new WaitUntil(() => _cylinder.OnLimitMin.Value);
            duration = Time.time - startTime;
            Assert.AreApproximatelyEqual(timeTo, duration, Utils.TEST_TOLERANCE, "Duration in minus direction isn't correct");
        }
        
        public struct MoveToLimitCase
        {
            public Cylinder.CylinderType Type;
            public bool JogMinus;
            public bool JogPlus;
            public bool ExpectedLimitMin;
            public bool ExpectedLimitMax;
            public float ExpectedValue;
        }

        public static IEnumerable<MoveToLimitCase> GetMoveToLimitCase()
        {
            yield return new MoveToLimitCase { Type = Cylinder.CylinderType.DoubleActing, JogMinus = false, JogPlus = false, ExpectedLimitMin = true, ExpectedLimitMax = false, ExpectedValue = 0f };
            yield return new MoveToLimitCase { Type = Cylinder.CylinderType.DoubleActing, JogMinus = true, JogPlus = false, ExpectedLimitMin = true, ExpectedLimitMax = false, ExpectedValue = 0f };
            yield return new MoveToLimitCase { Type = Cylinder.CylinderType.DoubleActing, JogMinus = true, JogPlus = true, ExpectedLimitMin = true, ExpectedLimitMax = false, ExpectedValue = 0f };
            yield return new MoveToLimitCase { Type = Cylinder.CylinderType.DoubleActing, JogMinus = false, JogPlus = true, ExpectedLimitMin = false, ExpectedLimitMax = true, ExpectedValue = 100f };
            
            yield return new MoveToLimitCase { Type = Cylinder.CylinderType.SingleActingPositive, JogMinus = false, JogPlus = false, ExpectedLimitMin = false, ExpectedLimitMax = true, ExpectedValue = 100f };
            yield return new MoveToLimitCase { Type = Cylinder.CylinderType.SingleActingPositive, JogMinus = true, JogPlus = false, ExpectedLimitMin = true, ExpectedLimitMax = false, ExpectedValue = 0f };
            yield return new MoveToLimitCase { Type = Cylinder.CylinderType.SingleActingPositive, JogMinus = true, JogPlus = true, ExpectedLimitMin = true, ExpectedLimitMax = false, ExpectedValue = 0f };
            yield return new MoveToLimitCase { Type = Cylinder.CylinderType.SingleActingPositive, JogMinus = false, JogPlus = true, ExpectedLimitMin = false, ExpectedLimitMax = true, ExpectedValue = 100f };
            
            yield return new MoveToLimitCase { Type = Cylinder.CylinderType.SingleActingNegative, JogMinus = false, JogPlus = false, ExpectedLimitMin = true, ExpectedLimitMax = false, ExpectedValue = 0f };
            yield return new MoveToLimitCase { Type = Cylinder.CylinderType.SingleActingNegative, JogMinus = true, JogPlus = false, ExpectedLimitMin = true, ExpectedLimitMax = false, ExpectedValue = 0f };
            yield return new MoveToLimitCase { Type = Cylinder.CylinderType.SingleActingNegative, JogMinus = true, JogPlus = true, ExpectedLimitMin = false, ExpectedLimitMax = true, ExpectedValue = 100f };
            yield return new MoveToLimitCase { Type = Cylinder.CylinderType.SingleActingNegative, JogMinus = false, JogPlus = true, ExpectedLimitMin = false, ExpectedLimitMax = true, ExpectedValue = 100f };
        }

        public static IEnumerable<float> GetMoveTimeCase()
        {
            yield return 0.2f;
            yield return 0.3f;
            yield return 0.4f;
            yield return 0.5f;
        }
    }
}

