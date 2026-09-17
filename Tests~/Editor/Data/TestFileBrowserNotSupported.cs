using System.Text.RegularExpressions;
using NUnit.Framework;
using OC.Data;
using UnityEngine;
using UnityEngine.TestTools;

namespace OC.Tests.Editor.Data
{
    public class TestFileBrowserNotSupported
    {
        private IFileBrowser _fileBrowser;

        [SetUp]
        public void SetUp()
        {
            _fileBrowser = new FileBrowserNotSupported();
        }

        private static void ExpectNotImplementedWarning(string method)
        {
            LogAssert.Expect(LogType.Warning, new Regex($"{method} is not implemented"));
        }

        [Test]
        public void OpenFilePanelReturnsEmpty()
        {
            ExpectNotImplementedWarning(nameof(IFileBrowser.OpenFilePanel));
            var paths = _fileBrowser.OpenFilePanel("Title", "", null, false);
            Assert.That(paths, Is.Not.Null);
            Assert.That(paths, Is.Empty);
        }

        [Test]
        public void OpenFolderPanelReturnsEmpty()
        {
            ExpectNotImplementedWarning(nameof(IFileBrowser.OpenFolderPanel));
            var paths = _fileBrowser.OpenFolderPanel("Title", "", false);
            Assert.That(paths, Is.Not.Null);
            Assert.That(paths, Is.Empty);
        }

        [Test]
        public void SaveFilePanelReturnsEmpty()
        {
            ExpectNotImplementedWarning(nameof(IFileBrowser.SaveFilePanel));
            var path = _fileBrowser.SaveFilePanel("Title", "", "Name", null);
            Assert.That(path, Is.Empty);
        }

        // The async members complete synchronously, so GetResult() also asserts they never hang.
        [Test]
        public void OpenFilePanelAsyncReturnsEmpty()
        {
            ExpectNotImplementedWarning(nameof(IFileBrowser.OpenFilePanel));
            var paths = _fileBrowser.OpenFilePanelAsync("Title", "", null, false).GetAwaiter().GetResult();
            Assert.That(paths, Is.Not.Null);
            Assert.That(paths, Is.Empty);
        }

        [Test]
        public void OpenFolderPanelAsyncReturnsEmpty()
        {
            ExpectNotImplementedWarning(nameof(IFileBrowser.OpenFolderPanel));
            var paths = _fileBrowser.OpenFolderPanelAsync("Title", "", false).GetAwaiter().GetResult();
            Assert.That(paths, Is.Not.Null);
            Assert.That(paths, Is.Empty);
        }

        [Test]
        public void SaveFilePanelAsyncReturnsEmpty()
        {
            ExpectNotImplementedWarning(nameof(IFileBrowser.SaveFilePanel));
            var path = _fileBrowser.SaveFilePanelAsync("Title", "", "Name", null).GetAwaiter().GetResult();
            Assert.That(path, Is.Empty);
        }
    }
}
