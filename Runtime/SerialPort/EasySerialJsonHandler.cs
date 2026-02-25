using System.IO;
using TMPro;
using UnityEngine;

namespace GAG.EasySerial
{
    public class EasySerialJsonHandler : MonoBehaviour
    {
        public EasySerialData EasySerialData;

        void OnEnable()
        {
            EasySerialEvents.DataReceived += OnSerialReceived;
            EasySerialEvents.SendJsonIndexRequested += SendJsonCommandIndex;
            EasySerialEvents.RequestJsonReload += LoadSerialData;
        }

        void OnDisable()
        {
            EasySerialEvents.DataReceived -= OnSerialReceived;
            EasySerialEvents.SendJsonIndexRequested -= SendJsonCommandIndex;
            EasySerialEvents.RequestJsonReload -= LoadSerialData;
        }

        private void SendJsonCommandIndex(int index)
        {
            // Safety checks
            if (EasySerialData == null || EasySerialData.Commands == null)
            {
                EasySerialLogger.Error("EasySerialData is not loaded.");
                return;
            }

            if (EasySerialData.Commands.Send == null ||
                index < 0 ||
                index >= EasySerialData.Commands.Send.Count)
            {
                EasySerialLogger.Warning($"Invalid Send index: {index}");
                return;
            }

            // Get command from JSON
            string command = EasySerialData.Commands.Send[index];

            // Debug log (optional but fits your style)
            EasySerialLogger.Highlight($"Send JSON Command [{index}] : {command}");

            // Send via EasySerial API (EVENT FLOW)
            EasySerial.Send(command);
        }

        void Start()
        {
            LoadSerialData();
        }

        public void LoadSerialData()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Serial Configurations.json");

            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                EasySerialData = JsonUtility.FromJson<EasySerialData>(json);
                
                if (EasySerialData?.SerialConfig != null)
                {
                    EasySerialEvents.RaiseSerialConfigLoaded(
                        EasySerialData.SerialConfig.Port,
                        EasySerialData.SerialConfig.BaudRate
                    );
                }
                
                EasySerialLogger.Log("Loaded Serial Configuration: " + json);
            }
            else
            {
                EasySerialLogger.Error("Serial Configuration file not found at: " + path);
                EasySerialData = new EasySerialData();
            }
        }

        void OnSerialReceived(string command)
        {
            if (EasySerialData?.Commands?.Receive == null)
            {
                EasySerialLogger.Error("Receive command list not loaded.");
                return;
            }

            // Validate against JSON
            if (EasySerialData.Commands.Receive.Contains(command))
            {
                EasySerialLogger.Log($"Validated JSON Receive: {command}");

                // Raise VALIDATED event
                EasySerialEvents.RaiseJsonCommandReceived(command);
            }
            else
            {
                EasySerialLogger.Warning("Unhandled Serial Received: " + command);
            }
        }
    }
}