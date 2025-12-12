using GAG.EasyUIConsole;
using System.IO;
using TMPro;
using UnityEngine;

namespace GAG.EasySerial
{
    /*
     * EasySerialAppManager
     * ---------------------
     * - Demonstrates how to integrate the EasySerial package.
     * - Loads serial configurations from a JSON file inside StreamingAssets.
     * - Allows sending custom commands or predefined JSON commands.
     * - Logs received serial commands through EasyUIConsole.
     *
     * Requirements:
     * - EasySerialHandler must exist in the scene (handles the SerialPort).
     * - EasyUIConsoleManager should be in the scene to view logs.
     * - Place “Serial Configurations.json” inside StreamingAssets.
     *
     * You can freely customize this class based on your project needs.
     */

    public class EasySerialAppManager : MonoBehaviour
    {
        public static EasySerialAppManager Instance;

        // Holds serial port configuration and command sets.
        public EasySerialData EasySerialData;

        // Used to send custom commands typed by the user.
        [SerializeField] TMP_InputField _cmdInputField;

        void Awake()
        {
            // Basic Singleton pattern (scene-level)
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnEnable()
        {
            // Subscribe to serial receive event
            EasySerialHandler.SerialReceived += OnSerialReceived;
        }

        void OnDisable()
        {
            // Clean up subscription
            EasySerialHandler.SerialReceived -= OnSerialReceived;
        }

        void Start()
        {
            LoadSerialData();
        }

        /// <summary>
        /// Loads serial configuration JSON from StreamingAssets.
        /// If the file is missing, default values will be used.
        /// </summary>
        public void LoadSerialData()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Serial Configurations.json");

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                EasySerialData = JsonUtility.FromJson<EasySerialData>(json);
                EasyUIConsoleManager.Instance.EasyLog("Loaded Serial Configuration: " + json);
            }
            else
            {
                EasyUIConsoleManager.Instance.EasyError("Serial Configuration file not found at: " + path);
                EasySerialData = new EasySerialData();   // fallback default data
            }

            if (EasySerialData == null)
            {
                EasyUIConsoleManager.Instance.EasyError("EasySerialData is null. Cannot update serial settings.");
                return;
            }

            // Apply loaded serial port settings
            UpdateSerialSettings(EasySerialData.SerialConfig.Port, EasySerialData.SerialConfig.BaudRate);
        }

        /// <summary>
        /// Applies port and baud rate to the EasySerialHandler.
        /// </summary>
        public void UpdateSerialSettings(string port, int baudRate)
        {
            EasySerialHandler.Instance.UpdateSerialSettings(port, baudRate);
        }

        /// <summary>
        /// Sends a custom command typed inside the UI input field.
        /// </summary>
        public void SendCustomCommand()
        {
            string command = _cmdInputField.text;

            if (string.IsNullOrWhiteSpace(command))
            {
                EasyUIConsoleManager.Instance.EasyWarning("Cannot send empty command.");
                return;
            }

            SendSerial(command);
        }

        /// <summary>
        /// Sends a predefined command stored inside the JSON file.
        /// </summary>
        public void SendJsonCommandByIndex(int index)
        {
            if (EasySerialData == null || EasySerialData.Commands == null) return;
            if (index < 0 || index >= EasySerialData.Commands.Send.Count) return;

            SendSerial(EasySerialData.Commands.Send[index]);
        }

        /// <summary>
        /// Sends a serial command using the EasySerial static interface.
        /// </summary>
        void SendSerial(string command)
        {
            EasySerial.SendSerial(command);
        }

        /// <summary>
        /// Handles all received serial commands.
        /// Compares incoming commands with the command list loaded from JSON.
        /// </summary>
        void OnSerialReceived(string command)
        {
            // Safety check
            if (EasySerialData == null || EasySerialData.Commands == null)
            {
                EasyUIConsoleManager.Instance.EasyError("Received data but EasySerialData is not loaded.");
                return;
            }

            // Example: checking the first 3 receive commands
            if (command == EasySerialData.Commands.Receive[0])
            {
                EasyUIConsoleManager.Instance.EasyLog("Serial 1st Received: " + command);
            }
            else if (command == EasySerialData.Commands.Receive[1])
            {
                EasyUIConsoleManager.Instance.EasyLog("Serial 2nd Received: " + command);
            }
            else if (command == EasySerialData.Commands.Receive[2])
            {
                EasyUIConsoleManager.Instance.EasyLog("Serial 3rd Received: " + command);
            }
            else
            {
                // Optional: catch all
                EasyUIConsoleManager.Instance.EasyWarning("Unhandled Serial Received: " + command);
            }
        }
    }
}
