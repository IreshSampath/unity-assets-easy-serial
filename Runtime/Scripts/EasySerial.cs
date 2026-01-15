using System;
using UnityEngine;

namespace GAG.EasySerial
{
    /// <summary>
    /// Public static API layer for EasySerial.
    /// This class exposes events and helper methods
    /// so other packages (UI, buttons, tools) can interact
    /// with EasySerial without direct dependencies.
    /// </summary>
    public static class EasySerial
    {
        /// <summary>
        /// Event used to request opening the UI console.
        /// UI-related packages (e.g. EasyUIConsole) can subscribe to this.
        /// </summary>
        public static event Action OpenConsoleRequested;

        /// <summary>
        /// Event raised when a serial JSON command is received and validated.
        /// Sends the command index as an int.
        /// </summary>
        public static event Action<int> ReceivedSerialJsonChecked;

        /// <summary>
        /// Safely raises the ReceivedSerialJsonChecked event.
        /// Call this from internal serial handlers.
        /// </summary>
        public static void RaiseReceivedSerialJsonChecked(int commandIndex)
        {
            ReceivedSerialJsonChecked?.Invoke(commandIndex);
        }

        /*
         * USAGE NOTES:
         * 
         * You can subscribe to serial events like this:
         * 
         * EasySerialHandler.SerialReceived += OnSerialReceived;
         * EasySerial.ReceivedSerialJsonChecked += OnReceivedSerialJsonChecked;
         * 
         * This keeps EasySerial independent from UI or game logic.
         */

        /// <summary>
        /// Sends a serial command using a JSON configuration index.
        /// </summary>
        public static void SendSerialByJsonIndex(int index)
        {
            EasySerialJsonHandler.Instance.SendJsonCommandByIndex(index);
        }

        /// <summary>
        /// Sends a raw serial command string.
        /// Includes a safety check to avoid null reference errors.
        /// </summary>
        public static void SendSerial(string cmd)
        {
            if (EasySerialHandler.Instance == null)
            {
                Debug.LogWarning("EasySerialHandler Instance is missing in the scene.");
                return;
            }

            EasySerialHandler.Instance.SendSerial(cmd);
        }

        /// <summary>
        /// Raises a request to open the console UI.
        /// UI packages can listen to this without EasySerial
        /// depending on them directly.
        /// </summary>
        public static void RaiseOpenConsole()
        {
            OpenConsoleRequested?.Invoke();
        }
    }
}
