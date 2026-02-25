using System;
using System.Collections.Generic;

[Serializable]
public class EasySerialData
{
    public SerialConfig SerialConfig = new SerialConfig();
    public CommandSet Commands = new CommandSet();
}

[Serializable]
public class SerialConfig
{
    public string Port = "COM3";
    public int BaudRate = 9600;
}

[Serializable]
public class CommandSet
{
    public List<string> Send = new List<string>();
    public List<string> Receive = new List<string>();
}
