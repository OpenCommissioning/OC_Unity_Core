using System;
using Cysharp.Threading.Tasks;

namespace OC.Data
{
    public static class FileBrowser
    {
        private static readonly IFileBrowser _browser;
        private static string _path;

        static FileBrowser() 
        {
#if UNITY_STANDALONE_WIN && (UNITY_EDITOR_WIN || !UNITY_EDITOR)
            _browser = new FileBrowserWindows();
#elif UNITY_EDITOR
            _browser = new FileBrowserEditor();
#else
            _browser = new FileBrowserNotSupported();
#endif
        }

        public static string[] OpenFilePanel(string title, string directory, string extension, bool multiselect) 
        {
            var extensions = string.IsNullOrEmpty(extension) ? null : new [] { new ExtensionFilter("", extension) };
            return OpenFilePanel(title, directory, extensions, multiselect);
        }

        public static string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            if (string.IsNullOrEmpty(directory)) directory = _path;
            var path = _browser.OpenFilePanel(title, directory, extensions, multiselect);
            if (path.Length == 0) return null;
             _path = path[0];
            return path;
        }
        
        public static async UniTask<string[]> OpenFilePanelAsync(string title, string directory, string extension, bool multiselect) 
        {
            var extensions = string.IsNullOrEmpty(extension) ? null : new [] { new ExtensionFilter("", extension) };
            return await OpenFilePanelAsync(title, directory, extensions, multiselect);
        }
        
        public static async UniTask<string[]> OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            if (string.IsNullOrEmpty(directory)) directory = _path;
            var path = await _browser.OpenFilePanelAsync(title, directory, extensions, multiselect);
            if (path.Length == 0) return null;
            _path = path[0];
            return path;
        }

        public static string[] OpenFolderPanel(string title, string directory, bool multiselect) 
        {
            return _browser.OpenFolderPanel(title, directory, multiselect);
        }
        
        public static async UniTask<string[]> OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb) 
        {
            return await _browser.OpenFolderPanelAsync(title, directory, multiselect);
        }

        public static string SaveFilePanel(string title, string directory, string defaultName , string extension) 
        {
            var extensions = string.IsNullOrEmpty(extension) ? null : new [] { new ExtensionFilter("", extension) };
            return SaveFilePanel(title, directory, defaultName, extensions);
        }

        public static string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions) 
        {
            return _browser.SaveFilePanel(title, directory, defaultName, extensions);
        }

        public static async UniTask<string> SaveFilePanelAsync(string title, string directory, string defaultName, string extension) 
        {
            var extensions = string.IsNullOrEmpty(extension) ? null : new [] { new ExtensionFilter("", extension) };
            return await SaveFilePanelAsync(title, directory, defaultName, extensions);
        }

        public static async UniTask<string> SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions) 
        {
            return await _browser.SaveFilePanelAsync(title, directory, defaultName, extensions);
        }
    }

    public struct ExtensionFilter
    {
        public readonly string Name;
        public readonly string[] Extensions;

        public ExtensionFilter(string filterName, params string[] filterExtensions)
        {
            Name = filterName;
            Extensions = filterExtensions;
        }
    }

    public interface IFileBrowser
    {
        string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect);
        string[] OpenFolderPanel(string title, string directory, bool multiselect);
        string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions);
        UniTask<string[]> OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect);
        UniTask<string[]> OpenFolderPanelAsync(string title, string directory, bool multiselect);
        UniTask<string> SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions);
    }
}
