using UnityEngine;
using UnityEditor;
using System.Xml.Linq;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using OC.Components;

namespace OC.Editor
{
    [InitializeOnLoad]
    public class TransformRestorer
    {
        private const string XML_PATH = "Assets/Temp_RestorableTransforms.xml";
        private static TransformRestorer _instance;
        
        private Dictionary<string, Matrix4x4> _recordedTransforms = new Dictionary<string, Matrix4x4>();

        public static TransformRestorer Instance
        {
            get
            {
                if (_instance == null) _instance = new TransformRestorer();
                return _instance;
            }
        }
        
        static TransformRestorer()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode) SavePositionsToXml();
            else if (state == PlayModeStateChange.EnteredEditMode) RestorePositionsFromXml();
        }
        
        private static string GetGlobalObjectId(GameObject gameObject)
        {
            GlobalObjectId id = GlobalObjectId.GetGlobalObjectIdSlow(gameObject);
            return id.ToString();
        }

        private static string MatrixToString(Matrix4x4 matrix)
        {
            float[] values = new float[]
            {
                matrix.m00, matrix.m01, matrix.m02, matrix.m03,
                matrix.m10, matrix.m11, matrix.m12, matrix.m13,
                matrix.m20, matrix.m21, matrix.m22, matrix.m23,
                matrix.m30, matrix.m31, matrix.m32, matrix.m33
            };
            return string.Join(",", values.Select(f => f.ToString("F8", CultureInfo.InvariantCulture)));
        }

        private static Matrix4x4 StringToMatrix(string matrixString)
        {
            var values = matrixString.Split(',').Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray();

            Matrix4x4 matrix;
            matrix.m00 = values[0];
            matrix.m01 = values[1];
            matrix.m02 = values[2];
            matrix.m03 = values[3];
            matrix.m10 = values[4];
            matrix.m11 = values[5];
            matrix.m12 = values[6];
            matrix.m13 = values[7];
            matrix.m20 = values[8];
            matrix.m21 = values[9];
            matrix.m22 = values[10];
            matrix.m23 = values[11];
            matrix.m30 = values[12];
            matrix.m31 = values[13];
            matrix.m32 = values[14];
            matrix.m33 = values[15];

            return matrix;
        }

        public void RecordTransform(GameObject gameObject)
        {
            var globalId = GetGlobalObjectId(gameObject);
            var currentTransform = gameObject.transform.GetMatrix();
            _recordedTransforms[globalId] = currentTransform;
            Debug.Log($"Recorded transform for '{(gameObject.name)}'");
        }

        private static void SavePositionsToXml()
        {
            if (Instance._recordedTransforms.Count == 0) return;
            var doc = new XDocument(new XElement("RestorablePositions"));
            var restorables = Object.FindObjectsOfType<MonoBehaviour>().OfType<ITransformRestorable>();

            foreach (var restorable in restorables)
            {
                var monoBehaviour = restorable as MonoBehaviour;
                var obj = monoBehaviour?.gameObject;
                var globalId = GetGlobalObjectId(obj);

                if (!Instance._recordedTransforms.ContainsKey(globalId))
                    continue;

                var matrix = Instance._recordedTransforms[globalId];
                var element = new XElement("Restorable",
                    new XAttribute("globalId", globalId),
                    new XAttribute("matrix", MatrixToString(matrix))
                );
                doc.Root?.Add(element);
            }

            doc.Save(XML_PATH);
            AssetDatabase.Refresh();
        }

        private static void RestorePositionsFromXml()
        {
            if (!File.Exists(XML_PATH)) return;
            var doc = XDocument.Load(XML_PATH);
            var restorables = Object.FindObjectsOfType<MonoBehaviour>().OfType<ITransformRestorable>();

            foreach (var restorable in restorables)
            {
                var monoBehavior = restorable as MonoBehaviour;
                var obj = monoBehavior?.gameObject;
                var globalId = GetGlobalObjectId(obj);

                if (!Instance._recordedTransforms.ContainsKey(globalId)) continue;
                
                var element = doc.Root?.Elements("Restorable").FirstOrDefault(e => e.Attribute("globalId")?.Value == globalId);
                if (element != null)
                {
                    var matrixAttr = element.Attribute("matrix");
                    if (matrixAttr != null)
                    {
                        var matrix = StringToMatrix(matrixAttr.Value);

                        var position = matrix.GetPosition();
                        var rotation = matrix.rotation;
                        var scale = matrix.lossyScale;

                        if (obj != null)
                        {
                            obj.transform.position = position;
                            obj.transform.rotation = rotation;
                            obj.transform.localScale = scale;

                            UnityEditor.EditorUtility.SetDirty(obj);
                            Debug.Log($"Restored transform for '{obj.name}' - Pos: {position}, Rot: {rotation.eulerAngles}, Scale: {scale}");
                        }
                    }
                }
            }
            File.Delete(XML_PATH);
            Instance._recordedTransforms.Clear();
            AssetDatabase.Refresh();
        }
    }
}