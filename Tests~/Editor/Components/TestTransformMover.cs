using NUnit.Framework;
using OC.Components;
using UnityEngine;
using UnityEngine.TestTools.Utils;

namespace OC.Tests.Editor.Components
{
    public class TestTransformMover
    {
        private const float DELTA_TIME = 0.1f;
        private GameObject _gameObject;
        private TransformMover _transformMover;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject();
            _transformMover = new TransformMover(_gameObject.transform);
        }

        [Test]
        public void ConstructorEmpty()
        {
            var position = new Vector3(1f, 1f, -1f);
            var rotation = Quaternion.Euler(10f, 10f, -10f);
            _gameObject.transform.SetPositionAndRotation(position, rotation);
            _transformMover = new TransformMover(_gameObject.transform);
            Assert.That(_gameObject.transform.localPosition, Is.EqualTo(position).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!"); 
            Assert.That(_gameObject.transform.localRotation, Is.EqualTo(rotation).Using(QuaternionEqualityComparer.Instance), "Local position isn´t correct!");
        }
        
        [Test]
        [TestCase(AxisDirection.X,AxisType.Translation,AxisControlMode.Position)]
        [TestCase(AxisDirection.Y,AxisType.Translation,AxisControlMode.Position)]
        [TestCase(AxisDirection.Z,AxisType.Translation,AxisControlMode.Position)]
        [TestCase(AxisDirection.X,AxisType.Rotation,AxisControlMode.Position)]
        [TestCase(AxisDirection.Y,AxisType.Rotation,AxisControlMode.Position)]
        [TestCase(AxisDirection.Z,AxisType.Rotation,AxisControlMode.Position)]
        [TestCase(AxisDirection.X,AxisType.Translation,AxisControlMode.Speed)]
        [TestCase(AxisDirection.Y,AxisType.Translation,AxisControlMode.Speed)]
        [TestCase(AxisDirection.Z,AxisType.Translation,AxisControlMode.Speed)]
        [TestCase(AxisDirection.X,AxisType.Rotation,AxisControlMode.Speed)]
        [TestCase(AxisDirection.Y,AxisType.Rotation,AxisControlMode.Speed)]
        [TestCase(AxisDirection.Z,AxisType.Rotation,AxisControlMode.Speed)]
        public void Constructor(AxisDirection direction, AxisType type, AxisControlMode control)
        {
            var position = new Vector3(1f, 1f, -1f);
            var rotation = Quaternion.Euler(10f, 10f, -10f);
            _gameObject.transform.SetPositionAndRotation(position, rotation);
            _transformMover = new TransformMover(_gameObject.transform, direction, type, control);
            Assert.That(_gameObject.transform.localPosition, Is.EqualTo(position).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!"); 
            Assert.That(_gameObject.transform.localRotation, Is.EqualTo(rotation).Using(QuaternionEqualityComparer.Instance), "Local position isn´t correct!");
            Assert.AreEqual(direction, _transformMover.Direction);
            Assert.AreEqual(type, _transformMover.AxisType);
            Assert.AreEqual(control, _transformMover.AxisControlMode);
        }
        
        [Test]
        [TestCase(AxisDirection.X,AxisType.Translation,AxisControlMode.Position)]
        [TestCase(AxisDirection.Y,AxisType.Translation,AxisControlMode.Position)]
        [TestCase(AxisDirection.Z,AxisType.Translation,AxisControlMode.Position)]
        [TestCase(AxisDirection.X,AxisType.Rotation,AxisControlMode.Position)]
        [TestCase(AxisDirection.Y,AxisType.Rotation,AxisControlMode.Position)]
        [TestCase(AxisDirection.Z,AxisType.Rotation,AxisControlMode.Position)]
        [TestCase(AxisDirection.X,AxisType.Translation,AxisControlMode.Speed)]
        [TestCase(AxisDirection.Y,AxisType.Translation,AxisControlMode.Speed)]
        [TestCase(AxisDirection.Z,AxisType.Translation,AxisControlMode.Speed)]
        [TestCase(AxisDirection.X,AxisType.Rotation,AxisControlMode.Speed)]
        [TestCase(AxisDirection.Y,AxisType.Rotation,AxisControlMode.Speed)]
        [TestCase(AxisDirection.Z,AxisType.Rotation,AxisControlMode.Speed)]
        public void SetConfig(AxisDirection direction, AxisType type, AxisControlMode control)
        {
            var position = new Vector3(1f, 1f, -1f);
            var rotation = Quaternion.Euler(10f, 10f, -10f);
            _gameObject.transform.SetPositionAndRotation(position, rotation);
            _transformMover = new TransformMover(_gameObject.transform);
            _transformMover.SetConfig(direction, type, control);
            Assert.That(_gameObject.transform.localPosition, Is.EqualTo(position).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!"); 
            Assert.That(_gameObject.transform.localRotation, Is.EqualTo(rotation).Using(QuaternionEqualityComparer.Instance), "Local position isn´t correct!");
            Assert.AreEqual(direction, _transformMover.Direction);
            Assert.AreEqual(type, _transformMover.AxisType);
            Assert.AreEqual(control, _transformMover.AxisControlMode);
        }

        [Test]
        [TestCase(AxisDirection.X,0)]
        [TestCase(AxisDirection.X,1)]
        [TestCase(AxisDirection.X,-1)]
        [TestCase(AxisDirection.Y,0)]
        [TestCase(AxisDirection.Y,1)]
        [TestCase(AxisDirection.Y,-1)]
        [TestCase(AxisDirection.Z,0)]
        [TestCase(AxisDirection.Z,1)]
        [TestCase(AxisDirection.Z,-1)]
        public void MoveTo(AxisDirection direction, float value)
        {
            var position = new Vector3(1f, 1f, -1f);
            var rotation = Quaternion.Euler(10f, 10f, -10f);
            _gameObject.transform.SetPositionAndRotation(position, rotation);
            _transformMover = new TransformMover(_gameObject.transform, direction,AxisType.Translation, AxisControlMode.Position);
            
            _transformMover.MoveTo(value);

            var result = position + rotation * Math.GetDirection(direction) * value;
            Assert.That(_gameObject.transform.localPosition, Is.EqualTo(result).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!");
            Assert.That(_gameObject.transform.localRotation, Is.EqualTo(rotation).Using(QuaternionEqualityComparer.Instance), "Local rotation isn´t correct!");
        }
        
        [Test]
        [TestCase(AxisDirection.X,0)]
        [TestCase(AxisDirection.X,10)]
        [TestCase(AxisDirection.X,-10)]
        [TestCase(AxisDirection.Y,0)]
        [TestCase(AxisDirection.Y,10)]
        [TestCase(AxisDirection.Y,-10)]
        [TestCase(AxisDirection.Z,0)]
        [TestCase(AxisDirection.Z,10)]
        [TestCase(AxisDirection.Z,-10)]
        public void RotateTo(AxisDirection direction, float value)
        {
            var position = new Vector3(1f, 1f, -1f);
            var rotation = Quaternion.Euler(10f, 10f, -10f);
            _gameObject.transform.SetPositionAndRotation(position, rotation);
            _transformMover = new TransformMover(_gameObject.transform, direction,AxisType.Rotation, AxisControlMode.Position);
            
            _transformMover.MoveTo(value);

            var result = rotation * Quaternion.Euler(Math.GetDirection(direction) * value);
            Assert.That(_gameObject.transform.localPosition, Is.EqualTo(position).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!");
            Assert.That(_gameObject.transform.localRotation, Is.EqualTo(result).Using(QuaternionEqualityComparer.Instance), "Local rotation isn´t correct!");
        }
        
        [Test]
        [TestCase(AxisDirection.X,0)]
        [TestCase(AxisDirection.X,10)]
        [TestCase(AxisDirection.X,-10)]
        [TestCase(AxisDirection.Y,0)]
        [TestCase(AxisDirection.Y,10)]
        [TestCase(AxisDirection.Y,-10)]
        [TestCase(AxisDirection.Z,0)]
        [TestCase(AxisDirection.Z,10)]
        [TestCase(AxisDirection.Z,-10)]
        public void Virtual(AxisDirection direction, float value)
        {
            var position = new Vector3(1f, 1f, -1f);
            var rotation = Quaternion.Euler(10f, 10f, -10f);
            _gameObject.transform.SetPositionAndRotation(position, rotation);
            _transformMover = new TransformMover(_gameObject.transform, direction,AxisType.Virtual, AxisControlMode.Position);
            
            _transformMover.MoveTo(value);
            Assert.That(_gameObject.transform.localPosition, Is.EqualTo(position).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!");
            Assert.That(_gameObject.transform.localRotation, Is.EqualTo(rotation).Using(QuaternionEqualityComparer.Instance), "Local rotation isn´t correct!");
        }
        
        [Test]
        [TestCase(AxisDirection.X,0)]
        [TestCase(AxisDirection.X,1)]
        [TestCase(AxisDirection.X,-1)]
        [TestCase(AxisDirection.Y,0)]
        [TestCase(AxisDirection.Y,1)]
        [TestCase(AxisDirection.Y,-1)]
        [TestCase(AxisDirection.Z,0)]
        [TestCase(AxisDirection.Z,1)]
        [TestCase(AxisDirection.Z,-1)]
        public void Translate(AxisDirection direction, float value)
        {
            var position = new Vector3(1f, 1f, -1f);
            var rotation = Quaternion.Euler(10f, 10f, -10f);
            _gameObject.transform.SetPositionAndRotation(position, rotation);
            _transformMover = new TransformMover(_gameObject.transform, direction,AxisType.Translation, AxisControlMode.Speed);
            
            _transformMover.MoveWithSpeed(value, DELTA_TIME);

            var expected = position + rotation * Math.GetDirection(direction) * value * DELTA_TIME;
            Assert.That(_gameObject.transform.localPosition, Is.EqualTo(expected).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!");
            Assert.That(_gameObject.transform.localRotation, Is.EqualTo(rotation).Using(QuaternionEqualityComparer.Instance), "Local rotation isn´t correct!");
        }
        
        [Test]
        [TestCase(AxisDirection.X,0)]
        [TestCase(AxisDirection.X,10)]
        [TestCase(AxisDirection.X,-10)]
        [TestCase(AxisDirection.Y,0)]
        [TestCase(AxisDirection.Y,10)]
        [TestCase(AxisDirection.Y,-10)]
        [TestCase(AxisDirection.Z,0)]
        [TestCase(AxisDirection.Z,10)]
        [TestCase(AxisDirection.Z,-10)]
        public void Rotate(AxisDirection direction, float value)
        {
            var position = new Vector3(1f, 1f, -1f);
            var rotation = Quaternion.Euler(10f, 10f, -10f);
            _gameObject.transform.SetPositionAndRotation(position, rotation);
            _transformMover = new TransformMover(_gameObject.transform, direction,AxisType.Rotation, AxisControlMode.Speed);
            
            _transformMover.MoveWithSpeed(value, 0.1f);

            var expected = rotation * Quaternion.Euler(Math.GetDirection(direction) * value * DELTA_TIME);
            Assert.That(_gameObject.transform.localPosition, Is.EqualTo(position).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!");
            Assert.That(_gameObject.transform.localRotation, Is.EqualTo(expected).Using(QuaternionEqualityComparer.Instance), "Local rotation isn´t correct!");
        }
        
        [Test]
        [TestCase(AxisDirection.X,0)]
        [TestCase(AxisDirection.X,1)]
        [TestCase(AxisDirection.X,-1)]
        [TestCase(AxisDirection.Y,0)]
        [TestCase(AxisDirection.Y,1)]
        [TestCase(AxisDirection.Y,-1)]
        [TestCase(AxisDirection.Z,0)]
        [TestCase(AxisDirection.Z,1)]
        [TestCase(AxisDirection.Z,-1)]
        public void VirtualSpeed(AxisDirection direction, float value)
        {
            var position = new Vector3(1f, 1f, -1f);
            var rotation = Quaternion.Euler(10f, 10f, -10f);
            _gameObject.transform.SetPositionAndRotation(position, rotation);
            _transformMover = new TransformMover(_gameObject.transform, direction,AxisType.Virtual, AxisControlMode.Speed);
            
            _transformMover.MoveWithSpeed(value, DELTA_TIME);

            Assert.That(_gameObject.transform.localPosition, Is.EqualTo(position).Using(Vector3EqualityComparer.Instance), "Local position isn´t correct!");
            Assert.That(_gameObject.transform.localRotation, Is.EqualTo(rotation).Using(QuaternionEqualityComparer.Instance), "Local rotation isn´t correct!");
        }
    }
}

