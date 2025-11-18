using UnityEditor;
using UnityEngine;
using System.IO;

namespace GAG.EasySerial.Editor
{
    public class EasySerialJsonGenerator : EditorWindow
    {
        EasySerialData _data = new EasySerialData();
        Vector2 _scroll;

        const string FILE_NAME = "Serial Configurations.json";

        [MenuItem("GAG/EasySerial/Serial Config Generator")]
        public static void Open()
        {
            GetWindow<EasySerialJsonGenerator>("EasySerial JSON Generator");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("EasySerial JSON Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            // Serial Settings
            EditorGUILayout.LabelField("Serial Port Configuration", EditorStyles.boldLabel);
            _data.SerialConfig.Port = EditorGUILayout.TextField("Port", _data.SerialConfig.Port);
            _data.SerialConfig.BaudRate = EditorGUILayout.IntField("Baud Rate", _data.SerialConfig.BaudRate);

            EditorGUILayout.Space();

            // Send Commands
            EditorGUILayout.LabelField("Send Commands", EditorStyles.boldLabel);
            if (_data.Commands.Send.Count == 0)
                _data.Commands.Send.Add("S0");

            for (int i = 0; i < _data.Commands.Send.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _data.Commands.Send[i] = EditorGUILayout.TextField($"Send[{i}]", _data.Commands.Send[i]);

                if (GUILayout.Button("X", GUILayout.Width(22)))
                {
                    _data.Commands.Send.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Send Command"))
                _data.Commands.Send.Add("S" + _data.Commands.Send.Count);

            EditorGUILayout.Space();

            // Receive Commands
            EditorGUILayout.LabelField("Receive Commands", EditorStyles.boldLabel);
            if (_data.Commands.Receive.Count == 0)
                _data.Commands.Receive.Add("R0");

            for (int i = 0; i < _data.Commands.Receive.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _data.Commands.Receive[i] = EditorGUILayout.TextField($"Receive[{i}]", _data.Commands.Receive[i]);

                if (GUILayout.Button("X", GUILayout.Width(22)))
                {
                    _data.Commands.Receive.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Receive Command"))
                _data.Commands.Receive.Add("R" + _data.Commands.Receive.Count);

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            // Generate button
            if (GUILayout.Button("Generate JSON File", GUILayout.Height(40)))
            {
                GenerateJson();
            }
        }

        void GenerateJson()
        {
            string folder = Application.streamingAssetsPath;

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string json = JsonUtility.ToJson(_data, true);
            string fullPath = Path.Combine(folder, FILE_NAME);

            File.WriteAllText(fullPath, json);

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Success",
                "Serial Configurations.json has been generated successfully!",
                "OK"
            );
        }
    }
}
