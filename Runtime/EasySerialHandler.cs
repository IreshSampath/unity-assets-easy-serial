using System;
using System.IO.Ports;
using UnityEngine;

namespace GAG.EasySerial
{
    public class EasySerialHandler : MonoBehaviour
    {
        SerialPort _serialPort;
        bool _isInitialized;

        [SerializeField] string _port = "COM3";
        [SerializeField] int _baudRate = 9600;
        
        bool _isConfigReady;
        bool _openRequested;
        
        void OnEnable()
        {
            EasySerialEvents.SerialConfigLoaded += ApplySerialConfig; 
            
            EasySerialEvents.OpenRequested += OpenSerial;
            EasySerialEvents.CloseRequested += ShutdownSerial;
            EasySerialEvents.SendRequested += SendSerial;
            EasySerialEvents.OpenConsoleRequested += OpenConsole;
        }

        void OnDisable()
        {
            EasySerialEvents.SerialConfigLoaded -= ApplySerialConfig;
            
            EasySerialEvents.OpenRequested -= OpenSerial;
            EasySerialEvents.CloseRequested -= ShutdownSerial;
            EasySerialEvents.SendRequested -= SendSerial;
            EasySerialEvents.OpenConsoleRequested -= OpenConsole;

            ShutdownSerial();
        }

        void Update()
        {
            if (_isInitialized)
                ReceiveSerial();
        }

        void ApplySerialConfig(string port,int baud)
        {
            ShutdownSerial();
            
            _port = port;
            _baudRate = baud;
            _isConfigReady = true;

            EasySerialLogger.Highlight($"Serial Config Ready → {port}@{baud}");

            // ⭐ Auto-open if someone already requested it
            if(_openRequested)
                TryOpenSerial();
        }
        
        void OpenSerial()
        {
            _openRequested = true;
            
            if(!_isConfigReady)
            {
                EasySerialLogger.Warning("Serial config not ready. Waiting for JSON...");
                return;
            }
            
            TryOpenSerial();
        }
        
        void TryOpenSerial()
        {
            if (_serialPort == null)
            {
                _serialPort = new SerialPort(_port,_baudRate)
                {
                    ReadTimeout = 1000,
                    WriteTimeout = 1000
                };
            }

            if (!_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                    _isInitialized = true;
                    _openRequested = false;
                    
                    EasySerialLogger.Highlight($"Opened Serial Port: {_port}");
                }
                catch(Exception e)
                {
                    EasySerialLogger.Error("Failed to open serial port: " + e.Message);
                }
            }
        }
        
        void SendSerial(string command)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    _serialPort.WriteLine(command);
                    EasySerialLogger.Log("Sent Serial Command: " + command);
                }
                catch (Exception e)
                {
                    EasySerialLogger.Error("Failed to send Serial command: " + e.Message);
                }
            }
            else
            {
                EasySerialLogger.Warning("Serial Port is not open.");
            }
        }

        void ReceiveSerial()
        {
            var sp = _serialPort;
            if (sp != null && sp.IsOpen && sp.BytesToRead > 0)
            {
                try
                {
                    string message = _serialPort.ReadLine()?.Trim();
                    if (!string.IsNullOrEmpty(message))
                    {
                        EasySerialLogger.Log("Received Serial Command: " + message);
                        EasySerialEvents.RaiseDataReceived(message);
                    }
                }
                catch (TimeoutException) { }
                catch (Exception e)
                {
                    EasySerialLogger.Error("Failed to receive Serial command: " + e.Message);
                }
            }
        }

        void OnApplicationQuit() => ShutdownSerial();

        void ShutdownSerial()
        {
            if (_serialPort == null) return;

            try
            {
                _isInitialized = false;
                _openRequested = false;

                if (_serialPort.IsOpen)
                {
                    _serialPort.DiscardInBuffer();
                    _serialPort.DiscardOutBuffer();
                    _serialPort.Close();

                    EasySerialLogger.Warning("Closed Serial Port");
                }

                _serialPort.Dispose();
                _serialPort = null;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
        
        public void OpenConsole()
        {
            //EasySerialLogger.OpenConsole();
        }
    }
}