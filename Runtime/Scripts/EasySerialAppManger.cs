using GAG.EasyUIConsole;
using System.IO;
using UnityEngine;

namespace GAG.EasySerial
{
    public class EasySerialAppManger : MonoBehaviour
    {
        public EasySerialData EasySerialData;

        void OnEnable()
        {
            EasySerialHandler.OnCommandReceived += ReceiveCommands;
        }
        void OnDisable()
        {
            EasySerialHandler.OnCommandReceived -= ReceiveCommands;
        }

        void Start()
        {
            LoadSerialData();
        }

        void LoadSerialData()
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
                EasySerialData = new EasySerialData(); // Load default values
            }

            if (EasySerialData == null)
            {
                EasyUIConsoleManager.Instance.EasyError("EasySerialData is null. Cannot update serial settings.");
                return;
            }
            else
            {
                EasySerialHandler.Instance.UpdateSerialSettings(EasySerialData.SerialConfig.Port, EasySerialData.SerialConfig.BaudRate);
            }
        }

        public void SendCommands(string command)
        {
            if (command == "0")
            {
                EasySerialHandler.Instance.SendCommand(EasySerialData.Commands.Command0Send);
            }
            else if (command == "1")
            {
                EasySerialHandler.Instance.SendCommand(EasySerialData.Commands.Command1Send);
            }
            else if (command == "2")
            {
                EasySerialHandler.Instance.SendCommand(EasySerialData.Commands.Command2Send);
            }
            else if (command == "3")
            {
                EasySerialHandler.Instance.SendCommand(EasySerialData.Commands.Command3Send);
            }
            else if (command == "4")
            {
                EasySerialHandler.Instance.SendCommand(EasySerialData.Commands.Command4Send);
            }
            else if (command == "5")
            {
                EasySerialHandler.Instance.SendCommand(EasySerialData.Commands.Command5Send);
            }
        }

        void ReceiveCommands(string command)
        {
            if (command == EasySerialData.Commands.Command0Receive)
            {
                EasyUIConsoleManager.Instance.EasyLog("Received: " + command);
            }
            else if (command == EasySerialData.Commands.Command1Receive)
            {
                EasyUIConsoleManager.Instance.EasyLog("Received: " + command);
            }
            else if (command == EasySerialData.Commands.Command2Receive)
            {
                EasyUIConsoleManager.Instance.EasyLog("Received: " + command);
            }
            else if (command == EasySerialData.Commands.Command3Receive)
            {
                EasyUIConsoleManager.Instance.EasyLog("Received: " + command);
            }
            else if (command == EasySerialData.Commands.Command4Receive)
            {
                EasyUIConsoleManager.Instance.EasyLog("Received: " + command);
            }
            else if (command == EasySerialData.Commands.Command5Receive)
            {
                EasyUIConsoleManager.Instance.EasyLog("Received: " + command);
            }
        }
    }
}
