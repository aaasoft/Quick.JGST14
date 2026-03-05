using System;

namespace Quick.JGST14.ElectronicGate;

public class TcpCommunicateContextOptions
{
    public string RemoteHost { get; set; }
    public int RemotePort { get; set; } = 9005;
    public string LocalListenIpAddress { get; set; } = "0.0.0.0";
    public int LocalListenPort { get; set; } = 8999;
    public int RecvBufferSize { get; set; } = 1024;
    public int SendBufferSize { get; set; } = 1024;
    public Action<string> Logger { get; set; }
}
