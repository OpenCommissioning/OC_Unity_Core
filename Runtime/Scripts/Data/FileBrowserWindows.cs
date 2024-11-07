#if UNITY_STANDALONE_WIN

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace OC.Data
{
    public class FileBrowserWindows : IFileBrowser
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        public string[] OpenFilePanel(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
        {
            var shellFilters = GetShellFilterFromFileExtensionList(extensions);
            if (multiselect)
            {
                var paths = ShellFileDialogs.FileOpenDialog.ShowMultiSelectDialog(GetActiveWindow(), title, directory, string.Empty, shellFilters, null);
                return paths == null || paths.Count == 0 ? Array.Empty<string>() : paths.ToArray();
            }
            else
            {
                var path = ShellFileDialogs.FileOpenDialog.ShowSingleSelectDialog(GetActiveWindow(), title, directory, string.Empty, shellFilters, null);
                return string.IsNullOrEmpty(path) ? Array.Empty<string>() : new[] { path };
            }
        }
        
#pragma warning disable CS1998
        public async UniTask<string[]> OpenFilePanelAsync(string title, string directory, ExtensionFilter[] extensions, bool multiselect)
#pragma warning restore CS1998
        {
            var path = OpenFilePanel(title, directory, extensions, multiselect);
            return path;
        }

        public string[] OpenFolderPanel(string title, string directory, bool multiselect)
        {
            var path = ShellFileDialogs.FolderBrowserDialog.ShowDialog(GetActiveWindow(), title, directory);
            return string.IsNullOrEmpty(path) ? Array.Empty<string>() : new[] { path };
        }

#pragma warning disable CS1998
        public async UniTask<string[]> OpenFolderPanelAsync(string title, string directory, bool multiselect)
#pragma warning restore CS1998
        {
            var path = OpenFolderPanel(title, directory, multiselect);
            return path;
        }

        public string SaveFilePanel(string title, string directory, string defaultName, ExtensionFilter[] extensions)
        {
            var finalFilename = "";
            if (!string.IsNullOrEmpty(directory))
            {
                finalFilename = GetDirectoryPath(directory);
            }

            if (!string.IsNullOrEmpty(defaultName))
            {
                finalFilename += defaultName;
            }

            var shellFilters = GetShellFilterFromFileExtensionList(extensions);
            if (shellFilters.Length > 0 && !string.IsNullOrEmpty(finalFilename))
            {
                var extension = $".{shellFilters[0].Extensions[0]}";
                if (!string.IsNullOrEmpty(extension) && !finalFilename.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase))
                    finalFilename += extension;
            }

            var path = ShellFileDialogs.FileSaveDialog.ShowDialog(GetActiveWindow(), title, directory, finalFilename, shellFilters, 0);
            return path;
        }

#pragma warning disable CS1998
        public async UniTask<string> SaveFilePanelAsync(string title, string directory, string defaultName, ExtensionFilter[] extensions)
#pragma warning restore CS1998
        {
            var path = SaveFilePanel(title, directory, defaultName, extensions);
            return path;
        }

        private static ShellFileDialogs.Filter[] GetShellFilterFromFileExtensionList(ExtensionFilter[] extensions)
        {
            var shellFilters = new List<ShellFileDialogs.Filter>();
            if (extensions != null)
            {
                foreach (var extension in extensions)
                {
                    if (extension.Extensions == null || extension.Extensions.Length == 0)
                        continue;

                    var displayName = extension.Name;
                    if (string.IsNullOrEmpty(displayName))
                    {
                        System.Text.StringBuilder extensionFormatted = new System.Text.StringBuilder();
                        foreach (var extensionStr in extension.Extensions)
                        {
                            if (extensionFormatted.Length > 0)
                                extensionFormatted.Append(";");
                            extensionFormatted.Append($"*.{extensionStr}");
                        }
                        displayName = $"({extensionFormatted})";
                    }
                    var filter = new ShellFileDialogs.Filter(displayName, extension.Extensions);
                    shellFilters.Add(filter);
                }
            }
            if (shellFilters.Count == 0)
            {
                shellFilters.AddRange(ShellFileDialogs.Filter.ParseWindowsFormsFilter(@"All files (*.*)|*.*"));
            }

            return shellFilters.ToArray();
        }

        private static string GetDirectoryPath(string directory)
        {
            var directoryPath = Path.GetFullPath(directory);
            if (!directoryPath.EndsWith("\\"))
            {
                directoryPath += "\\";
            }
            if (Path.GetPathRoot(directoryPath) == directoryPath)
            {
                return directory;
            }
            return Path.GetDirectoryName(directoryPath) + Path.DirectorySeparatorChar;
        }
    }
}

#endif