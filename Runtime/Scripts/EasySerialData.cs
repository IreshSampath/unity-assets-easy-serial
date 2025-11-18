using System;

[Serializable]
public class EasySerialData
{
    public SerialConfig SerialConfig = new SerialConfig();
    public Commands Commands = new Commands();
}

[Serializable]
public class SerialConfig
{
    public string Port = "COM3";
    public int BaudRate = 9600;
}

[Serializable]
public class Commands
{
    public string Command0Send = "S0";
    public string Command1Send = "S1";
    public string Command2Send = "S2";
    public string Command3Send = "S3";
    public string Command4Send = "S4";
    public string Command5Send = "S5";

    public string Command0Receive = "R0";
    public string Command1Receive = "R1";
    public string Command2Receive = "R2";
    public string Command3Receive = "R3";
    public string Command4Receive = "R4";
    public string Command5Receive = "R5";
}
