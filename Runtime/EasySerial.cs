using System;
using UnityEngine;

namespace GAG.EasySerial
{
    public static class EasySerial
    {
        // RECEIVE
        public static event Action<string> OnReceived;
        public static event Action<string> OnJsonReceived;
        
        static EasySerial()
        {
            EasySerialEvents.DataReceived += HandleDataReceived;
            EasySerialEvents.JsonCommandReceived += HandleJsonReceived;
        }

        static void HandleDataReceived(string data)
        {
            OnReceived?.Invoke(data);
        }

        static void HandleJsonReceived(string json)
        {
            OnJsonReceived?.Invoke(json);
        }
        
        // Config
        public static void ReloadJson()
        {
            EasySerialEvents.RaiseRequestJsonReload();
        }
        
        // CONTROL
        public static void Open()
        {
            EasySerialEvents.RaiseOpenRequested();
        }

        public static void Close()
        {
            EasySerialEvents.RaiseCloseRequested();
        }
        
        public static void OpenConsole()
        {
#if EASY_UICONSOLE
            EasySerialEvents.RaiseOpenConsoleRequested();
            #else
            Debug.Log("Requires EasyUIConsole");
            #endif
        }
        
        // SEND
        public static void Send(string text)
        {
            EasySerialEvents.RaiseSendRequested(text);
        }
        
        public static void SendByJsonIndex(int index)
        {
            EasySerialEvents.RaiseSendJsonIndexRequested(index);
        }
    }
}