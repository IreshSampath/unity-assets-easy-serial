using UnityEngine;

namespace GAG.EasySerial
{
    public static class EasySerial
    {
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
