using UnityEngine;
using TMPro;
using GAG.EasySerial;

public class EasySerialDemo : MonoBehaviour
{
    [SerializeField] TMP_InputField _inputField;

    // ---------------------------------------------------
    // RECEIVE
    // ---------------------------------------------------
    void OnEnable()
    {
        EasySerial.OnReceived += OnData;
        EasySerial.OnJsonReceived += OnJson;
    }

    void OnDisable()
    {
        EasySerial.OnReceived -= OnData;
        EasySerial.OnJsonReceived -= OnJson;
    }
    
    void OnData(string msg)
    {
        print($"RX: {msg}");
    }

    void OnJson(string json)
    {
        print($"JSON: {json}");
    }
    
    // ---------------------------------------------------
    // BUTTON METHODS (PUBLIC → Inspector OnClick)
    // ---------------------------------------------------

    public void ReloadJson()
    {
        print("Reload Json");
        EasySerial.ReloadJson();
    }

    public void OpenSerial()
    {
        print("Open Serial");
        EasySerial.Open();
    }

    public void CloseSerial()
    {
        print("Close Serial");
        EasySerial.Close();
    }

    public void OpenConsole()
    {
        print("Open Console");
        EasySerial.OpenConsole();
    }

    public void SendCustom()
    {
        if (string.IsNullOrEmpty(_inputField.text))
        {
            EasySerialLogger.Warning("Input empty");
            print("Input empty");
            
            return;
        }
        print($"Send: {_inputField.text}");
        EasySerial.Send(_inputField.text);
    }

    public void SendJson(int index)
    {
        print($"Send Json: {index}");
        EasySerial.SendByJsonIndex(index);
    }
}