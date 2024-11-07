using System;
using Cysharp.Threading.Tasks;

namespace OC.Data
{
    public static class FileBrowser
    {
        private static readonly IFileBrowser Browser;
        private static string _path;

        static FileBrowser() 
        {
#if UNITY_STANDALONE_WIN
            Browser = new FileBrowserWindows();
#elif UNITY_EDITOR
            Browser = new FileBrowserEditor();
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
            var pathes = Browser.OpenFilePanel(title, directory, extensions, multiselect);
            if (pathes.Length == 0) return null;
             _path = pathes[0];
            return pathes;
        }
        
        public static async UniTask<string[]> OpenFilePanelAsync(string title, string directory, string extension, bool multiselect) 
        {
            var extensions = string.IsNullOrEmpty(extension) ? null : new [] { new ExtensionFilter("", extension) };
            return await OpenFilePanelAsync(title, directory, extensions, multiselect);
        }
        
        public static async UniTask<string[]> OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            if (string.IsNullOrEmpty(directory)) directory = _path;
            var pathes = await Browser.OpenFilePanelAsync(title, directory, extensions, multiselect);
            if (pathes.Length == 0) return null;
            _path = pathes[0];
            return pathes;
        }

        public static string[] OpenFolderPanel(string title, string directory, bool multiselect) 
        {
            return Browser.OpenFolderPanel(title, directory, multiselect);
        }
        
        public static async UniTask<string[]> OpenFolderPanelAsync(string title, string directory, bool multiselect, Action<string[]> cb) 
        {
            return await Browser.OpenFolderPanelAsync(title, directory, multiselect);
        }

        public static string SaveFilePanel(string title, string directory, string defaultName , string extension) 
        {
            var extensions = string.IsNullOrEmpty(extension) ? null : new [] { new ExtensionFilter("", extension) };
            return SaveFilePanel(title, directory, defaultName, extensions);
        }

        public static string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions) 
        {
            return Browser.SaveFilePanel(title, directory, defaultName, extensions);
        }

        public static async UniTask<string> SaveFilePanelAsync(string title, string directory, string defaultName, string extension) 
        {
            var extensions = string.IsNullOrEmpty(extension) ? null : new [] { new ExtensionFilter("", extension) };
            return await SaveFilePanelAsync(title, directory, defaultName, extensions);
        }

        public static async UniTask<string> SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions) 
        {
            return await Browser.SaveFilePanelAsync(title, directory, defaultName, extensions);
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
