using UnityEngine;
using UnityEditor;
using System.IO;

public class SamplesAssetPostprocessor : AssetPostprocessor
{
    const string FILE_NAME = "Type0.xml";
    const string SOURCE_PATH = @"Packages/com.open-commissioning.core/Samples~/Demo/Data/RFID";
    const string DESTINATION_PATH = "Data/RFID"; // relative to StreamingAssets

    private static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        if (importedAssets.Length == 0 || !importedAssets[0].StartsWith($"Assets/Samples/Open Commissioning")) return;
        CopyTemplateFile();
    }

    private static void CopyTemplateFile()
    {
        if (!Directory.Exists(Application.streamingAssetsPath))
            Directory.CreateDirectory(Application.streamingAssetsPath);

        var targetDirectory = Path.Combine(Application.streamingAssetsPath, DESTINATION_PATH);
        if (!Directory.Exists(targetDirectory)) Directory.CreateDirectory(targetDirectory);

        var sourcePath = Path.Combine(Path.GetFullPath(SOURCE_PATH), FILE_NAME);
        var destinationPath = Path.Combine(targetDirectory, FILE_NAME);

        try
        {
            if (!File.Exists(sourcePath)) return;
            if (!File.Exists(destinationPath))
                File.Copy(sourcePath, destinationPath, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error copying file to StreamingAssets: {e.Message}");
        }
    }
}