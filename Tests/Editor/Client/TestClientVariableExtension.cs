using NUnit.Framework;
using OC.Communication;

namespace OC.Editor.Tests.Client
{
    public class TestClientVariableExtension
    {
        [Test]
        [TestCase("Machine&3", ExpectedResult = false)]
        [TestCase("_FG01", ExpectedResult = false)]
        [TestCase("0001", ExpectedResult = false)]
        [TestCase("FG 01", ExpectedResult = false)]
        [TestCase("FG-01", ExpectedResult = false)]
        [TestCase("FG01", ExpectedResult = true)]
        [TestCase("FG_01", ExpectedResult = true)]
        public bool IsVariableNameValid(string name)
        {
            return ClientVariableExtension.IsVariableNameValid(name);
        }

        [Test]
        [TestCase("MAIN.ST01.FG01.Cylinder_1", ExpectedResult = "MAIN.ST01.FG01.Cylinder_1")]
        [TestCase("MAIN.++ST01.FG01.Cylinder_1", ExpectedResult = "MAIN.`++ST01`.FG01.Cylinder_1")]
        [TestCase("MAIN.++ST01.+&FG01.--Cylinder_1", ExpectedResult = "MAIN.`++ST01`.`+&FG01`.`--Cylinder_1`")]
        public string GetCompatiblePath(string input)
        {
            return input.GetCompatiblePath();
        }
    }
}