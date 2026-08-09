#if UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using UnityEditor;

namespace OC.Data
{
    public class FileBrowserEditor : IFileBrowser
    {
        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            var path = extensions == null
                ? EditorUtility.OpenFilePanel(title, directory, "")
                : EditorUtility.OpenFilePanelWithFilters(title, directory, GetFilterFromFileExtensionList(extensions));
            return string.IsNullOrEmpty(path) ? Array.Empty<string>() : new[] { path };
        }

        public async UniTask<string[]> OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            var paths = OpenFilePanel(title, directory, extensions, multiselect);
            // The panel blocks the main thread while it is open. Yield a frame so the caller
            // resumes on the next player loop tick instead of inside the blocked stack.
            await UniTask.Yield();
            return paths;
        }

        public string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            var path = EditorUtility.OpenFolderPanel(title, directory, "");
            return string.IsNullOrEmpty(path) ? Array.Empty<string>() : new[] { path };
        }

        public async UniTask<string[]> OpenFolderPanelAsync(string title, string directory, bool multiselect)
        {
            var paths = OpenFolderPanel(title, directory, multiselect);
            await UniTask.Yield();
            return paths;
        }

        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            var ext = extensions != null ? extensions[0].Extensions[0] : "";
            var name = string.IsNullOrEmpty(ext) ? defaultName : defaultName + "." + ext;
            return EditorUtility.SaveFilePanel(title, directory, name, ext);
        }

        public async UniTask<string> SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            var path = SaveFilePanel(title, directory, defaultName, extensions);
            await UniTask.Yield();
            return path;
        }

        private static string[] GetFilterFromFileExtensionList(ExtensionFilter[] extensions)
        {
            var filters = new string[extensions.Length * 2];
            for (var i = 0; i < extensions.Length; i++)
            {
                filters[(i * 2)] = extensions[i].Name;
                filters[(i * 2) + 1] = string.Join(",", extensions[i].Extensions);
            }

            return filters;
        }
    }
}
#endif
