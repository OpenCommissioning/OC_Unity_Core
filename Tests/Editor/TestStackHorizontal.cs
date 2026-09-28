using System.IO;
using NUnit.Framework;
using OC.VisualElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OC.Tests.Editor
{
    public class TestStackHorizontal
    {
        [Test]
        public void UxmlCreatesStackWithExistingNameAndChildContent()
        {
            var path = AssetDatabase.GenerateUniqueAssetPath("Assets/StackHorizontalTest.uxml");
            try
            {
                File.WriteAllText(path,
                    "<ui:UXML xmlns:ui=\"UnityEngine.UIElements\" xmlns:oc=\"OC.VisualElements\">" +
                    "<oc:StackHorizontal name=\"stack\" tooltip=\"test tooltip\">" +
                    "<ui:Label name=\"child\" text=\"Content\" />" +
                    "</oc:StackHorizontal></ui:UXML>");
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                Assert.That(asset, Is.Not.Null);
                var root = asset.CloneTree();
                var stack = root.Q<StackHorizontal>("stack");
                Assert.That(stack, Is.Not.Null);
                Assert.That(stack.tooltip, Is.EqualTo("test tooltip"));
                Assert.That(stack.ClassListContains("stack-horizontal"), Is.True);
                Assert.That(stack.styleSheets.Contains(Resources.Load<StyleSheet>("StyleSheet/oc-default")), Is.True);
                Assert.That(stack.childCount, Is.EqualTo(1));
                Assert.That(stack.Q<Label>("child").text, Is.EqualTo("Content"));
            }
            finally
            {
                AssetDatabase.DeleteAsset(path);
            }
        }

        [Test]
        public void AddKeepsChildAndSetsFlexGrow()
        {
            var stack = new StackHorizontal();
            var child = new VisualElement();
            stack.Add(child);
            Assert.That(child.parent, Is.SameAs(stack));
            Assert.That(child.style.flexGrow.value, Is.EqualTo(1f));
        }
    }
}
