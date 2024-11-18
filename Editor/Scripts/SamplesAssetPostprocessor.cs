using System;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace OC.Editor
{
    public class SamplesAssetPostprocessor : AssetPostprocessor
    {
        private const string DATA_PATH = "Demo/Data/RFID";
        private const string SAMPLE_IDENTIFIER = "Samples/Open Commissioning/";
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (Directory.Exists(Path.Combine(Application.streamingAssetsPath, DATA_PATH))) return;
            if (importedAssets.Length == 0 || !importedAssets[0].StartsWith("Assets/"+SAMPLE_IDENTIFIER)) return;

            var splits = importedAssets[0].Split('/');
            var versionString = splits[3];

            var sourceFullPath = Path.Combine(Path.GetFullPath(Application.dataPath), "Samples/Open Commissioning/", versionString, DATA_PATH);

            MoveTemplateFiles(sourceFullPath);

            var deletePath = Directory.GetParent(sourceFullPath)!.FullName;
            FileUtil.DeleteFileOrDirectory(deletePath);
            FileUtil.DeleteFileOrDirectory(deletePath + ".meta");
            AssetDatabase.Refresh();
        }

        private static void MoveTemplateFiles(string sourceFullPath)
        {
            var targetDirectory = Path.Combine(Application.streamingAssetsPath, DATA_PATH);

            if (!Directory.Exists(targetDirectory)) Directory.CreateDirectory(targetDirectory);

            try
            {
                foreach (var file in Directory.GetFiles(sourceFullPath))
                {
                    var targetFileName = Path.Combine(targetDirectory, Path.GetFileName(file));
                    File.Move(file, targetFileName);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error moving files to StreamingAssets: {e.Message}");
            }
        }
    }
}