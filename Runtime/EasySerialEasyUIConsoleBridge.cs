#if EASY_UICONSOLE
using GAG.EasyUIConsole;
using UnityEngine;

namespace GAG.EasySerial
{
    public class EasySerialEasyUIConsoleBridge : MonoBehaviour
    {
        void OnEnable()
        {
            EasySerialLogger.LogMsg += OnLog;
            EasySerialEvents.OpenConsoleRequested += OpenConsole;
        }

        void OnDisable()
        {
            EasySerialLogger.LogMsg -= OnLog;
            EasySerialEvents.OpenConsoleRequested -= OpenConsole;
        }

        void OnLog(string msg, EasySerialLogType type)
        {
            switch (type)
            {
                case EasySerialLogType.Highlight: EasyUIC.Highlight(msg); break;
                case EasySerialLogType.Warning: EasyUIC.Warning(msg); break;
                case EasySerialLogType.Error: EasyUIC.Error(msg); break;
                default: EasyUIC.Log(msg); break;
            }
        }

        void OpenConsole()
        {
            EasyUIC.OpenConsole();
        }
    }
}
#endif