using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;
using Cysharp.Threading.Tasks;
using OC.Data;
using OC.MaterialFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace OC
{
    public static class SnapshotManager
    {
        private const string TAG = "<b><color=#b78cf9>Snapshot Manager</color></b>";
        
        public static void Create()
        {
            var sceneName = SceneManager.GetActiveScene().name.Replace(" ", "_").Replace(".", "_");
            var file = FileBrowser.SaveFilePanel("Create Snapshot", "", GetDefaultSnapshotName(sceneName), "zip");
            if (string.IsNullOrEmpty(file))
            {
                return;
            }
            
            CreateSnapshot(file);
        }
        
        public static void Load()
        {
            var paths = FileBrowser.OpenFilePanel("Load Snapshot", "", "zip", false);
            if (paths == null || paths.Length == 0)
            {
                return;
            }
            
            var path = paths[0];
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            
            LoadSnapshot(path).Forget();
        }

        private static async UniTask CreateWithAsyncDialog()
        {
            var sceneName = SceneManager.GetActiveScene().name.Replace(" ", "_").Replace(".", "_");
            var file = await UniTask.RunOnThreadPool(() => FileBrowser.SaveFilePanelAsync("Create Snapshot", "", GetDefaultSnapshotName(sceneName), "zip"));
            await UniTask.SwitchToMainThread();
            
            if (string.IsNullOrEmpty(file))
            {
                return;
            } 
            CreateSnapshot(file);
        }

        private static async UniTask LoadWithAsyncDialog()
        {
            var paths = await UniTask.RunOnThreadPool(() => FileBrowser.OpenFilePanelAsync("Load Snapshot", "", "zip", false));
            await UniTask.SwitchToMainThread();
            if (paths == null || paths.Length == 0)
            {
                return;
            }
            
            var path = paths[0];
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            
            await LoadSnapshot(path);
        }

        private static void CreateSnapshot(string file)
        {
            if (string.IsNullOrEmpty(file)) return;
            if (Directory.Exists(file))
            {
                Logging.Logger.Log(LogType.Error, $"{TAG} File already exists at {file}");
                return;
            }
            
            var tempFolder = Directory.CreateDirectory(file.Replace(".zip",""));

            try
            {
                var sceneSnapshotFile = tempFolder + @"\SceneSnapshot.xml";
            
                var sceneSnapshot = new SceneSnapshot
                {
                    PayloadsDescription = SerializeEntites(),
                    DevicesDescription =  SerializeDevices(),
                    ComponentsDescription = SerializeComponents()
                };

                var serializer = new XmlSerializer(typeof(SceneSnapshot));
                using (var writer = new StreamWriter(File.Open(sceneSnapshotFile, FileMode.Create)))
                {
                    serializer.Serialize(writer, sceneSnapshot);
                    writer.Close();
                }

                SaveTagDirectories(ProductDataDirectoryManager.Instance, tempFolder.FullName);
                
                var archiveName = tempFolder.FullName + ".zip";
                if (File.Exists(archiveName)) File.Delete(archiveName);
                ZipFile.CreateFromDirectory(tempFolder.FullName, archiveName);
                Logging.Logger.Log(LogType.Log, $"{TAG} Snapshot is created at {file}");
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, $"{exception.Message} \n{exception.StackTrace}");
            }
            finally
            {
                Directory.Delete(tempFolder.FullName,true);
            }
        }

#pragma warning disable CS1998
        private static async UniTask LoadSnapshot(string file)
#pragma warning restore CS1998
        {
            if (string.IsNullOrEmpty(file)) return;
            
            var tempFolder = Directory.CreateDirectory(file.Replace(".zip", ""));

            try
            {
                ZipFile.ExtractToDirectory(file, tempFolder.FullName);

                var sceneSnapshotFile = tempFolder + @"\SceneSnapshot.xml";
                var serializer = new XmlSerializer(typeof(SceneSnapshot));
                using var streamReader = new StreamReader(File.OpenRead(sceneSnapshotFile));
                var data = serializer.Deserialize(streamReader) as SceneSnapshot;
                streamReader.Close();

                DeserializeDevices(data);
                await UniTask.DelayFrame(5);
                Physics.SyncTransforms();
                DeserializePayloads(data);
                LoadTagDirectories(tempFolder.FullName);
                
                DeserializeComponents(data);

                Logging.Logger.Log(LogType.Log, $"{TAG} Snapshot successfully loaded from {file}");
            }
            catch (Exception exception)
            {
                Logging.Logger.Log(LogType.Error, $"{exception.Message} \n{exception.StackTrace}");
            }
            finally
            {
                Directory.Delete(tempFolder.FullName, true);
            }
        }
        
        private static List<PayloadDescription> SerializeEntites()
        {
            var payloads = Pool.Instance.PoolManager.Payloads.Select(item => item.Value).OrderBy(payload => payload.Category).ToList();
            return payloads.Select(payload => new PayloadDescription(payload)).ToList();
        }

        private static List<DeviceDescription> SerializeDevices()
        {
            var devices = Object.FindObjectsByType<MonoBehaviour>(sortMode: FindObjectsSortMode.InstanceID).OfType<IDeviceMetadata>().ToList();
            return (from device in devices where device.Link.Enable select new DeviceDescription(device)).ToList();
        }

        private static List<ComponentDescription> SerializeComponents()
        {
            var components = Object.FindObjectsByType<MonoBehaviour>(sortMode: FindObjectsSortMode.InstanceID).OfType<IComponentMetadata>().ToList();
            return components.Select(component => new ComponentDescription(component)).ToList();
        }
        
        private static void DeserializePayloads(SceneSnapshot sceneSnapshot)
        {
            var payloads = new List<Payload>();

            foreach (var payloadDescription in sceneSnapshot.PayloadsDescription)
            {
                var payload = Pool.Instance.PoolManager.Instantiate(payloadDescription);
                if (payload is null) continue;
                payloads.Add(payload);
            }

            foreach (var payload in payloads)
            {
                var parent = Pool.Instance.PoolManager.FindPayload(payload.ParentUniqueId);
                if (parent is not null) payload.SetParent(parent.transform);
            }
        }

        private static void DeserializeDevices(SceneSnapshot sceneSnapshot)
        {
            foreach (var device in sceneSnapshot.DevicesDescription)
            {
                device.Deserialize();
            }
        }

        private static void DeserializeComponents(SceneSnapshot sceneSnapshot)
        {
            foreach (var component in sceneSnapshot.ComponentsDescription)
            {
                component.Deserialize();
            }
        }
        
        private static void SaveTagDirectories(ProductDataDirectoryManager productDataDirectoryManager, string path)
        {
            foreach (var tagDirectory in productDataDirectoryManager.ProductDataDirectories)
            {
                var directory = Directory.CreateDirectory(path + $@"\{tagDirectory.Name}");
                CopyFiles(tagDirectory.GetValidPath(), directory.FullName);
            }
        }

        private static void LoadTagDirectories(string path)
        {
            var folders = Directory.GetDirectories(path);
            foreach (var folder in folders)
            {
                var localFolder = folder.Split(@"\").Last();
                foreach (var dataPath in ProductDataDirectoryManager.Instance.ProductDataDirectories.Where(dataPath => dataPath.Name == localFolder))
                {
                    CopyFiles(folder, dataPath.GetValidPath());
                }
            }
        }

        private static void CopyFiles(string sourcePath, string targetPath)
        {
            foreach (var dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            foreach (var newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
        
        private static string GetDefaultSnapshotName(string sceneName)
        {
            var date = DateTime.Now.ToString("yyyyMMdd_HHmm");
            return $"{sceneName}_{date}";
        }
    }
}
