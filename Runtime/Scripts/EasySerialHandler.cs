using GAG.EasyUIConsole;
using System;
using System.IO.Ports;
using System.Linq;
using UnityEngine;

namespace GAG.EasySerial
{
    public class EasySerialHandler : MonoBehaviour
    {
        // Change the API Compatibility Level to .NET Framework in Player Settings
        // to use "using System.IO.Ports;"

        public static EasySerialHandler Instance;

        public static event Action<string> SerialReceived;
        public static void RaiseSerialReceived(string command) => SerialReceived?.Invoke(command);

        SerialPort _serialPort;
        bool _isInitialized = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Update()
        {
            if (_isInitialized)
            {
                ReceiveSerial();
            }
        }

        public void UpdateSerialSettings(string port, int baudRate)
        {
            if (_serialPort == null)
            {
                _serialPort = new SerialPort(port, baudRate);

                _serialPort.ReadTimeout = 1000;
                _serialPort.WriteTimeout = 1000;
            }

            if (!_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                    EasyUIConsoleManager.Instance.EasyHiglight("Opened Serial Port: " + port);
                    _isInitialized = true;
                }
                catch (System.Exception e)
                {
                    EasyUIConsoleManager.Instance.EasyError("Failed to open serial port: " + e.Message);
                }
            }
            else
            {
                EasyUIConsoleManager.Instance.EasyWarning("Serial Port is already open: " + _serialPort.PortName);
            }
        }

        public void SendSerial(string command)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    _serialPort.WriteLine(command);
                    EasyUIConsoleManager.Instance.EasyLog("Sent Serial Command: " + command);
                }
                catch (System.Exception e)
                {
                    EasyUIConsoleManager.Instance.EasyError("Failed to send Serial command: " + e.Message);
                }
            }
            else
            {
                EasyUIConsoleManager.Instance.EasyWarning("Serial Port is not open. Cannot send Serial command.");
            }
        }

        void ReceiveSerial()
        {
            if (_serialPort != null && _serialPort.IsOpen && _serialPort.BytesToRead > 0)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    if (!string.IsNullOrEmpty(message))
                    {
                        EasyUIConsoleManager.Instance.EasyLog("Received Serial Command: " + message);
                        RaiseSerialReceived(message);
                    }
                }
                catch (TimeoutException)
                {
                    // Ignore timeout exceptions, as they are expected when no data is available
                }
                catch (System.Exception e)
                {
                    EasyUIConsoleManager.Instance.EasyError("Failed to receive Serial command: " + e.Message);
                }
            }
        }

        public void OpenConsole()
        {
            GameObject panel = Resources.FindObjectsOfTypeAll<GameObject>()
    .FirstOrDefault(go => go.name == "Panel Console");

            if (panel == null)
            {
                Debug.LogWarning(
                    "Panel Console not found in hierarchy. Download and install EasyUIConsole package and import the EasyUIConsole prefab"
                );
                return;
            }

            panel.SetActive(true);
        }

        void OnApplicationQuit()
        {
            //if (_serialPort != null && _serialPort.IsOpen)
            //{
            //    _serialPort.Close();
            //    EasyUIConsoleManager.Instance.EasyWarning("Closed Serial Port");
            //}
#if !UNITY_EDITOR
            Process.GetCurrentProcess().Kill();
#endif
        }
    }
}
