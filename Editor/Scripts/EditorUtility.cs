using System.IO;
using UnityEditor;
using UnityEngine;

namespace OC.Editor
{
    public static class EditorUtility
    {
        public static void CreateScreenshot()
        {
            var path = UnityEditor.EditorUtility.OpenFolderPanel("Create Screenshot", "", "Screenshot.png");
            if (string.IsNullOrEmpty(path)) return;

            var fileName = Path.Combine(path, "Screenshot.png").Replace("\\", "/");
            fileName = GetNextAvailableFilename(fileName);
            ScreenCapture.CaptureScreenshot(fileName);
            Logging.Logger.Log($"{fileName} created!");
            Logging.Logger.Log($"{Directory.GetCurrentDirectory()}");
        }

        [MenuItem ("GameObject/Distance #x")]
        public static void Distance()
        {
            var distance = Vector3.Distance(Selection.transforms[0].position, Selection.transforms[1].position);
            Debug.Log($"{Selection.transforms[0].name} <> {Selection.transforms[1].name} => Distance: <color=lightblue>{distance}</color>");
        }
        
        [MenuItem ("GameObject/Distance #x", true)]
        public static bool DistanceValidation()
        {
            return Selection.transforms.Length == 2;
        }

        public static string GetNextAvailableFilename(string fileName)
        {
            if (!File.Exists(fileName)) return fileName;

            string availableFilename;
            var fileNameIndex = 1;
            do
            {
                fileNameIndex += 1;
                availableFilename = CreateNumberedFilename(fileName, fileNameIndex);
            } 
            while (File.Exists(availableFilename));

            return availableFilename;
        }
        
        private static string CreateNumberedFilename(string fileName, int number)
        {
            var path = Path.GetDirectoryName(fileName);
            var name = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);
            return $"{path}\\{name}{number}{extension}";
        }
    }
}
