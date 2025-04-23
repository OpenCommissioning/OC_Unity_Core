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
        [TestCase("Machine&3", ExpectedResult = "Machine3")]
        [TestCase("_FG01", ExpectedResult = "A_FG01")]
        [TestCase("0001", ExpectedResult = "A0001")]
        [TestCase("FG 01", ExpectedResult = "FG_01")]
        [TestCase("FG-01", ExpectedResult = "FG_01")]
        [TestCase("FG01", ExpectedResult = "FG01")]
        [TestCase("FG_01", ExpectedResult = "FG_01")]
        public string CorrectVariableName(string input)
        {
            return ClientVariableExtension.CorrectVariableName(input);
        }
    }
}