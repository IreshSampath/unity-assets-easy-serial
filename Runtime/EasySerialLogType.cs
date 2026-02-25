using System;
using UnityEngine;

namespace GAG.EasySerial
{
    public enum EasySerialLogType
    {
        Log,
        Highlight,
        Warning,
        Error
    }

    public static class EasySerialLogger
    {
        public static event Action<string, EasySerialLogType> LogMsg;

        public static void Print(string msg, EasySerialLogType type = EasySerialLogType.Log)
        {
#if UNITY_EDITOR
            Debug.Log(msg);
#endif
            LogMsg?.Invoke(msg, type);
        }

        public static void Log(string msg) => Print(msg, EasySerialLogType.Log);
        public static void Highlight(string msg) => Print(msg, EasySerialLogType.Highlight);
        public static void Warning(string msg) => Print(msg, EasySerialLogType.Warning);
        public static void Error(string msg) => Print(msg, EasySerialLogType.Error);
    }
}