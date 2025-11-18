using GAG.EasyUIConsole;
using System.IO;
using UnityEngine;

namespace GAG.EasySerial
{
    // This is a sample manager to demonstrate EasySerial usage.
    // It loads serial configurations from a JSON file in StreamingAssets. You should move this json file
    // to your StreamingAssets folder and customize the commands as needed.
    // or else you can send commands directly via SendSerial method.
    // and handles sending/receiving serial commands.
    // You can customize this class as needed for your application.
    // Make sure to have EasySerialHandler in the scene for serial communication.

    public class EasySerialAppManager : MonoBehaviour
    {
        public EasySerialData EasySerialData;

        void OnEnable()
        {
            EasySerialHandler.SerialReceived += OnSerialReceived;
        }
        void OnDisable()
        {
            EasySerialHandler.SerialReceived -= OnSerialReceived;
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

        public void SendByIndex(int index)
        {
            if (EasySerialData == null || EasySerialData.Commands == null) return;
            if (index < 0 || index >= EasySerialData.Commands.Send.Count) return;

            SendSerial(EasySerialData.Commands.Send[index]);
        }

        public void SendSerial(string command)
        {
            EasySerial.SendSerial(command);
        }

        void OnSerialReceived(string command)
        {
            if (command == EasySerialData.Commands.Receive[0])
            {
                EasyUIConsoleManager.Instance.EasyLog("Received: " + command);
            }
            else if (command == EasySerialData.Commands.Receive[1])
            {
                EasyUIConsoleManager.Instance.EasyLog("Received: " + command);
            }
            else if (command == EasySerialData.Commands.Receive[2])
            {
                EasyUIConsoleManager.Instance.EasyLog("Received: " + command);
            }
        }
    }
}
