using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OC.Data
{
    public class FileBrowserNotSupported : IFileBrowser
    {
        private const string TAG = "<b><color=#b78cf9>File Browser</color></b>";

        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            LogNotSupported(nameof(OpenFilePanel));
            return Array.Empty<string>();
        }

#pragma warning disable CS1998
        public async UniTask<string[]> OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
#pragma warning restore CS1998
        {
            return OpenFilePanel(title, directory, extensions, multiselect);
        }

        public string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            LogNotSupported(nameof(OpenFolderPanel));
            return Array.Empty<string>();
        }

#pragma warning disable CS1998
        public async UniTask<string[]> OpenFolderPanelAsync(string title, string directory, bool multiselect)
#pragma warning restore CS1998
        {
            return OpenFolderPanel(title, directory, multiselect);
        }

        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            LogNotSupported(nameof(SaveFilePanel));
            return string.Empty;
        }

#pragma warning disable CS1998
        public async UniTask<string> SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions)
#pragma warning restore CS1998
        {
            return SaveFilePanel(title, directory, defaultName, extensions);
        }

        private static void LogNotSupported(string method)
        {
            Logging.Logger.LogWarning($"{TAG} {method} is not implemented on {Application.platform}");
        }
    }
}
