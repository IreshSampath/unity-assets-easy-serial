#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GAG.EasySerial.Editor
{
    [InitializeOnLoad]
    public static class EasySerialAutoOpen
    {
        const string PREF_KEY = "GAG_EASYSERIAL_FIRST_INSTALL";

        static EasySerialAutoOpen()
        {
            // Delay call so Unity finishes loading assemblies
            EditorApplication.delayCall += TryOpenWindow;
        }

        static void TryOpenWindow()
        {
            // Already opened once → don't show again
            if (EditorPrefs.GetBool(PREF_KEY, false))
                return;

            // Mark as opened
            EditorPrefs.SetBool(PREF_KEY, true);

            // Open your control panel
            EasySerialControlPanel.Open();

            Debug.Log("[EasySerial] First install detected → Opening Control Panel.");
        }
    }
}
#endif