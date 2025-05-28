using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools.Utils;

namespace OC.Tests
{
    public static class Utils
    {
        public const float TEST_TOLERANCE = 0.5f;
        public const float TEST_SCALE = 1f;

        // https://docs.unity3d.com/ScriptReference/Vector3.Equals.html
        // Use the == operator to test two vectors for approximate equality. 

        //public static void AreEqual(Vector3 expected, Vector3 actual, string message)
        //{
        //    Assert.True(actual.Equals(expected), message);
        //}
        //
        //public static void AreEqual(Vector3 expected, Vector3 actual)
        //{
        //    Utils.AreEqual(expected, actual, null);
        //}

        public static void AreApproximatelyEqual(Quaternion expected, Quaternion actual, string message)
        {
            Assert.True(actual == expected, message);
        }

        public static void AreApproximatelyEqual(Quaternion expected, Quaternion actual)
        {
            AreApproximatelyEqual(expected, actual, null);
        }

        public static void AreEqual(Quaternion expected, Quaternion actual, string message)
        {
            Assert.True(QuaternionEqualityComparer.Instance.Equals(expected, actual), message);
        }

        public static void AreEqual(Quaternion expected, Quaternion actual)
        {
            AreEqual(expected, actual, null);
        }

        public static void AreApproximatelyEqual(Vector3 expected, Vector3 actual, float tolerance)
        {
            AreApproximatelyEqual(expected, actual, tolerance, null);
        }

        public static void AreApproximatelyEqual(Vector3 expected, Vector3 actual, float tolerance, string message)
        {
            float distance = Vector3.Distance(expected, actual);
           
            Assert.That(distance, Is.LessThanOrEqualTo(tolerance), message);
        }

        public static void AreApproximatelyEqual(Vector3 expected, Vector3 actual, string message)
        { 
            Assert.True(actual == expected, message);
        }

        public static void AreApproximatelyEqual(Vector3 expected, Vector3 actual)
        {
            AreApproximatelyEqual(expected, actual, null);
        }

        public static void AreEqual(Vector3 expected, Vector3 actual, string message)
        {
            Assert.AreEqual(expected, actual, message);
        }

        public static void AreEqual(Vector3 expected, Vector3 actual)
        {
            AreEqual(expected, actual, null);
        }
    }
}
