#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace GAG.EasySerial.Editor
{
    public class EasySerialControlPanel : EditorWindow
    {
        // ======================================================
        // CONFIG DATA
        // ======================================================
        EasySerialData _data = new EasySerialData();
        Vector2 _scroll;

        const string FILE_NAME = "Serial Configurations.json";

        // ======================================================
        // UI
        // ======================================================
        int _tabIndex;
        readonly string[] _tabs = { "Setup", "Config", "Utilities" };

        // ======================================================
        // EASY UI CONSOLE INTEGRATION
        // ======================================================
        const string EASY_UICONSOLE_PACKAGE = "com.ireshsampath.unity-assets.easy-ui-console";
        const string EASY_UICONSOLE_REPO = "https://github.com/IreshSampath/unity-assets-easy-ui-console.git";
        const string EASY_UICONSOLE_DEFINE = "EASY_UICONSOLE";

        AddRequest _installRequest;

        // ======================================================
        // MENU
        // ======================================================
        [MenuItem("Tools/GAG/EasySerial")]
        public static void Open()
        {
            GetWindow<EasySerialControlPanel>("EasySerial");
        }

        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("EasySerial Control Panel", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            _tabIndex = GUILayout.Toolbar(_tabIndex, _tabs);

            EditorGUILayout.Space();

            switch (_tabIndex)
            {
                case 0: DrawSetupTab(); break;
                case 1: DrawConfigTab(); break;
                case 2: DrawUtilitiesTab(); break;
            }
        }

        // ======================================================
        // 🧩 SETUP TAB
        // ======================================================
        void DrawSetupTab()
        {
            DrawWindowsSerialPortEnvironment();
            EditorGUILayout.Space(12);
            DrawEasyUIConsoleIntegration();
        }

        void DrawWindowsSerialPortEnvironment()
        {
            EditorGUILayout.LabelField("Windows SerialPort Environment", EditorStyles.boldLabel);

            var group = BuildTargetGroup.Standalone;
            var backend = PlayerSettings.GetScriptingBackend(group);
            var api = PlayerSettings.GetApiCompatibilityLevel(group);

            EditorGUILayout.HelpBox(
                $"Current Backend: {backend}\nAPI Level: {api}",
                MessageType.Info);

            if (backend != ScriptingImplementation.Mono2x)
            {
                EditorGUILayout.HelpBox(
                    "Mono backend recommended for SerialPort support.",
                    MessageType.Warning);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Apply Windows SerialPort Settings", GUILayout.Height(36)))
                ApplyWindowsSettings();
        }

        void ApplyWindowsSettings()
        {
            var group = BuildTargetGroup.Standalone;

            PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.Mono2x);
            PlayerSettings.SetApiCompatibilityLevel(group, ApiCompatibilityLevel.NET_4_6);

            EditorUtility.DisplayDialog(
                "EasySerial",
                "Windows SerialPort environment applied successfully.",
                "OK");

            Debug.Log("[EasySerial] Mono + .NET Framework applied.");
        }

        void DrawEasyUIConsoleIntegration()
        {
            EditorGUILayout.LabelField("EasyUIConsole Integration", EditorStyles.boldLabel);

            bool installed = IsEasyUIConsoleInstalled();
            bool defineEnabled = HasDefine(EASY_UICONSOLE_DEFINE);

            EditorGUILayout.HelpBox(
                $"Installed: {(installed ? "YES" : "NO")}\nDefine ({EASY_UICONSOLE_DEFINE}): {(defineEnabled ? "ENABLED" : "DISABLED")}",
                installed ? MessageType.Info : MessageType.Warning);

            EditorGUILayout.BeginHorizontal();

            using (new EditorGUI.DisabledScope(_installRequest != null))
            {
                if (GUILayout.Button("Install EasyUIConsole", GUILayout.Height(30)))
                    InstallEasyUIConsole();
            }

            using (new EditorGUI.DisabledScope(!installed))
            {
                if (GUILayout.Button("Import Sample", GUILayout.Height(30)))
                    ImportEasyUIConsoleSample();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Enable Define"))
                AddDefine(EASY_UICONSOLE_DEFINE);

            if (GUILayout.Button("Disable Define"))
                RemoveDefine(EASY_UICONSOLE_DEFINE);

            EditorGUILayout.EndHorizontal();

            if (_installRequest != null)
                EditorGUILayout.HelpBox("Installing EasyUIConsole... Please wait.", MessageType.Info);
        }

        bool IsEasyUIConsoleInstalled()
        {
            return UnityEditor.PackageManager.PackageInfo
                .GetAllRegisteredPackages()
                .Any(p => p.name == EASY_UICONSOLE_PACKAGE);
        }

        void InstallEasyUIConsole()
        {
            _installRequest = Client.Add(EASY_UICONSOLE_REPO);
            EditorApplication.update += InstallProgress;
        }

        void InstallProgress()
        {
            if (_installRequest == null || !_installRequest.IsCompleted)
                return;

            EditorApplication.update -= InstallProgress;

            if (_installRequest.Status == StatusCode.Success)
            {
                Debug.Log("[EasySerial] EasyUIConsole installed.");
                AddDefine(EASY_UICONSOLE_DEFINE);
                Repaint();
            }
            else
            {
                Debug.LogError("[EasySerial] Install failed: " + _installRequest.Error.message);
            }

            _installRequest = null;
        }

        void ImportEasyUIConsoleSample()
        {
            var pkg = UnityEditor.PackageManager.PackageInfo
                .GetAllRegisteredPackages()
                .FirstOrDefault(p => p.name == EASY_UICONSOLE_PACKAGE);

            if (pkg == null)
            {
                Debug.LogWarning("[EasySerial] EasyUIConsole not installed.");
                return;
            }

            var samplesEnum = Sample.FindByPackage(pkg.name, pkg.version);
            if (samplesEnum == null)
            {
                Debug.LogWarning("[EasySerial] Sample list is null.");
                return;
            }

            var samples = samplesEnum.ToList();
            if (samples.Count == 0)
            {
                Debug.LogWarning("[EasySerial] No samples found.");
                return;
            }

            // Pick preferred sample by name, otherwise first
            Sample chosen = samples[0];
            foreach (var s in samples)
            {
                if (!string.IsNullOrEmpty(s.displayName) &&
                    s.displayName.Contains("EasyUIConsole"))
                {
                    chosen = s;
                    break;
                }
            }

            chosen.Import(Sample.ImportOptions.OverridePreviousImports);

            Debug.Log($"[EasySerial] Imported sample: {chosen.displayName}");
        }

        // ======================================================
        // DEFINE HELPERS
        // ======================================================
        bool HasDefine(string symbol)
        {
            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var named = NamedBuildTarget.FromBuildTargetGroup(group);

            var defines = PlayerSettings.GetScriptingDefineSymbols(named)
                .Split(';')
                .Select(d => d.Trim())
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();

            return defines.Contains(symbol);
        }

        void AddDefine(string symbol)
        {
            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var named = NamedBuildTarget.FromBuildTargetGroup(group);

            var defines = PlayerSettings.GetScriptingDefineSymbols(named)
                .Split(';')
                .Select(d => d.Trim())
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();

            if (!defines.Contains(symbol))
                defines.Add(symbol);

            PlayerSettings.SetScriptingDefineSymbols(named, string.Join(";", defines));
            Debug.Log($"[EasySerial] Added define '{symbol}' for {group}");
        }

        void RemoveDefine(string symbol)
        {
            var group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var named = NamedBuildTarget.FromBuildTargetGroup(group);

            var defines = PlayerSettings.GetScriptingDefineSymbols(named)
                .Split(';')
                .Select(d => d.Trim())
                .Where(d => !string.IsNullOrEmpty(d))
                .ToList();

            defines.RemoveAll(d => d == symbol);

            PlayerSettings.SetScriptingDefineSymbols(named, string.Join(";", defines));
            Debug.Log($"[EasySerial] Removed define '{symbol}' for {group}");
        }

        // ======================================================
        // 🧩 CONFIG TAB
        // ======================================================
        void DrawConfigTab()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorGUILayout.LabelField("Serial Port Configuration", EditorStyles.boldLabel);

            EnsureDefaults();

            _data.SerialConfig.Port =
                EditorGUILayout.TextField("Port", _data.SerialConfig.Port);

            _data.SerialConfig.BaudRate =
                EditorGUILayout.IntField("Baud Rate", _data.SerialConfig.BaudRate);

            EditorGUILayout.Space();

            DrawSendCommands();
            EditorGUILayout.Space();
            DrawReceiveCommands();

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate JSON File", GUILayout.Height(36)))
                GenerateJson();
        }

        void EnsureDefaults()
        {
            //if (_data.SerialConfig == null)
              //  _data.SerialConfig = new EasySerialConfig();

          //  if (_data.Commands == null)
               // _data.Commands = new EasySerialCommands();

            if (_data.Commands.Send == null)
                _data.Commands.Send = new List<string>();

            if (_data.Commands.Receive == null)
                _data.Commands.Receive = new List<string>();

            if (string.IsNullOrEmpty(_data.SerialConfig.Port))
                _data.SerialConfig.Port = "COM3";

            if (_data.SerialConfig.BaudRate <= 0)
                _data.SerialConfig.BaudRate = 9600;
        }

        void DrawSendCommands()
        {
            EditorGUILayout.LabelField("Send Commands", EditorStyles.boldLabel);

            if (_data.Commands.Send.Count == 0)
                _data.Commands.Send.Add("S0");

            for (int i = 0; i < _data.Commands.Send.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                _data.Commands.Send[i] =
                    EditorGUILayout.TextField($"Send[{i}]", _data.Commands.Send[i]);

                if (GUILayout.Button("X", GUILayout.Width(22)))
                {
                    _data.Commands.Send.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Send Command"))
                _data.Commands.Send.Add("S" + _data.Commands.Send.Count);
        }

        void DrawReceiveCommands()
        {
            EditorGUILayout.LabelField("Receive Commands", EditorStyles.boldLabel);

            if (_data.Commands.Receive.Count == 0)
                _data.Commands.Receive.Add("R0");

            for (int i = 0; i < _data.Commands.Receive.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                _data.Commands.Receive[i] =
                    EditorGUILayout.TextField($"Receive[{i}]", _data.Commands.Receive[i]);

                if (GUILayout.Button("X", GUILayout.Width(22)))
                {
                    _data.Commands.Receive.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("+ Add Receive Command"))
                _data.Commands.Receive.Add("R" + _data.Commands.Receive.Count);
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
                $"{FILE_NAME} generated successfully!",
                "OK");

            Debug.Log("[EasySerial] JSON Generated → " + fullPath);
        }

        // ======================================================
        // 🧩 UTILITIES TAB
        // ======================================================
        void DrawUtilitiesTab()
        {
            EditorGUILayout.LabelField("Utilities", EditorStyles.boldLabel);

            if (GUILayout.Button("Open StreamingAssets Folder", GUILayout.Height(30)))
            {
                string path = Application.streamingAssetsPath;

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                EditorUtility.RevealInFinder(path);
            }
        }
    }
}
#endif