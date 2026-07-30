using UnityEngine;

namespace OC.Tests
{
    public static class TestUtils
    {
        public static T CreateComponent<T>() where T : Component
        {
            var gameObject = new GameObject();
            return gameObject.AddComponent<T>();
        }
    }
}