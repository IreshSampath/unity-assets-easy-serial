using System;

namespace GAG.EasySerial
{
    public static class EasySerialEvents
    {
        // Config
        public static event Action RequestJsonReload;
        public static event Action<string,int> SerialConfigLoaded;
        
        // CONTROL
        public static event Action OpenRequested;
        public static event Action CloseRequested;
        public static event Action OpenConsoleRequested;
        
        // SEND
        public static event Action<string> SendRequested;
        public static event Action<int> SendJsonIndexRequested;
        
        // RECEIVE
        public static event Action<string> DataReceived;
        public static event Action<string> JsonCommandReceived;
        
        public static void RaiseRequestJsonReload() => RequestJsonReload?.Invoke();
        public static void RaiseSerialConfigLoaded(string port,int baud) => SerialConfigLoaded?.Invoke(port,baud);
        
        public static void RaiseOpenRequested() => OpenRequested?.Invoke();
        public static void RaiseCloseRequested() => CloseRequested?.Invoke();
        public static void RaiseOpenConsoleRequested() => OpenConsoleRequested?.Invoke();
        
        public static void RaiseSendJsonIndexRequested(int index) => SendJsonIndexRequested?.Invoke(index);
        public static void RaiseJsonCommandReceived(string data) => JsonCommandReceived?.Invoke(data);

        public static void RaiseSendRequested(string cmd) => SendRequested?.Invoke(cmd);
        public static void RaiseDataReceived(string data) => DataReceived?.Invoke(data);
    }
}