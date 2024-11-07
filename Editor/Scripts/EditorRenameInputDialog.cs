using System;
using UnityEditor;
using UnityEngine;

namespace OC.Editor
{
    public class EditorRenameInputDialog : EditorWindow
    {
        public Action OkButtonClicked;
        public string Description;
        public string InputText;
        public string OkButtonLabel;
        public string CancelButtonLabel;
        public bool InitializedPosition;
        public bool ShouldClose;

        private void OnGUI()
        {
            var e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    case KeyCode.Escape:
                        ShouldClose = true;
                        e.Use();
                        break;
                    case KeyCode.Return:
                    case KeyCode.KeypadEnter:
                        OkButtonClicked?.Invoke();
                        ShouldClose = true;
                        e.Use();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (ShouldClose) Close();

            var rect = EditorGUILayout.BeginVertical();
            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField(Description);
            EditorGUILayout.Space(8);
            GUI.SetNextControlName("inText");
            InputText = EditorGUILayout.TextField("", InputText);
            GUI.FocusControl("inText");
            EditorGUILayout.Space(12);

            var r = EditorGUILayout.GetControlRect();
            r.width /= 2;
            if (GUI.Button(r, OkButtonLabel))
            {
                OkButtonClicked?.Invoke();
                ShouldClose = true;
            }

            r.x += r.width;
            if (GUI.Button(r, CancelButtonLabel))
            {
                InputText = null;
                ShouldClose = true;
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.EndVertical();


            if (rect.width != 0 && minSize != rect.size)
            {
                minSize = maxSize = rect.size;
            }

            if (!InitializedPosition)
            {
                var mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                position = new Rect(mousePos.x + 32, mousePos.y, position.width, position.height);
                InitializedPosition = true;
            }
        }

        /// <summary>
        /// Returns text player entered, or null if player cancelled the dialog.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="inputText"></param>
        /// <param name="okButton"></param>
        /// <param name="cancelButton"></param>
        /// <returns></returns>
        public static string Show(string title, string description, string inputText, string okButton = "OK", string cancelButton = "Cancel")
        {
            string result = null;
            var window = CreateInstance<EditorRenameInputDialog>();
            window.titleContent = new GUIContent(title);
            window.Description = description;
            window.InputText = inputText;
            window.OkButtonLabel = okButton;
            window.CancelButtonLabel = cancelButton;
            window.OkButtonClicked += () => result = window.InputText;
            window.ShowModal();
            return result;
        }
    }
}
