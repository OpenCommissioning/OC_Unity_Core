using NUnit.Framework;
using OC.Components;
using UnityEngine;
using UnityEngine.TestTools.Utils;

namespace OC.Tests.Editor.Common
{
    public class TestMath
    {
        [Test]
        [TestCase(AxisDirection.X,1,0,0)]
        [TestCase(AxisDirection.Y,0,1,0)]
        [TestCase(AxisDirection.Z,0,0,1)]
        public void GetDirection(AxisDirection axisDirection, float x, float y, float z)
        {
            var direction = new Vector3(x, y, z);
            Assert.That(Math.GetDirection(axisDirection), Is.EqualTo(direction).Using(Vector3EqualityComparer.Instance), "Direction result isn´t correct!"); 
        }
    }
}
