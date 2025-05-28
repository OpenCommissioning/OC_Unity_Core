using System;
using Cysharp.Threading.Tasks;
using OC.Communication.TwinCAT;
using UnityEditor;
using UnityEngine;

namespace OC.Editor
{
    [CustomEditor(typeof(TcAdsClient),false)]
    public class TcAdsClientInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var client = (TcAdsClient) target;

            GUILayout.Space(10);
            if (GUILayout.Button("Connect"))
            {
                client.Connect();
            }

            if (GUILayout.Button("Disconnect"))
            {
                client.Disconnect();
            }

            if (GUILayout.Button("Create Configuration"))
            {
                ProjectTreeFactory.CreateAndSave(client);
            }

            if (GUILayout.Button("Update TwinCAT Project"))
            {
                SendConfigAsync(client).Forget();
            }
        }

#pragma warning disable CS1998
        private async UniTask SendConfigAsync(Component root)
#pragma warning restore CS1998
        {
            try
            {
                var projectTree = ProjectTreeFactory.Create(root);
                var connection = new Assistant.Connection();
                await connection.SendConfigAsync(projectTree).AsUniTask();
                Logging.Logger.Log("Project Tree sent to Assistant", root);
            }
            catch (Exception exception)
            {
                Logging.Logger.LogError(exception);
            }
        }
    }
}
