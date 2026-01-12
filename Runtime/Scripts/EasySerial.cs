using System;
using UnityEngine;

namespace GAG.EasySerial
{
    public static class EasySerial
    {
        // Subscribe to all serial receive event
        //EasySerialHandler.SerialReceived += OnSerialReceived;

        // Subscribe to json checked serial receive event
        //EasySerial.ReceivedSerialJsonChecked += OnReceivedSerialJsonChecked;

        public static event Action<int> ReceivedSerialJsonChecked;
        public static void RaiseReceivedSerialJsonChecked(int commandIndex) => ReceivedSerialJsonChecked?.Invoke(commandIndex);

        public static void SendSerialByJsonIndex(int index)
        {
            EasySerialJsonHandler.Instance.SendJsonCommandByIndex(index);
        }

        public static void SendSerial(string cmd)
        {
            if (EasySerialHandler.Instance == null)
            {
                Debug.LogWarning("EasySerialHandler Instance is missing in the scene.");
                return;
            }

            EasySerialHandler.Instance.SendSerial(cmd);
        }


    }
}
