namespace Quick.JGST14.ElectronicGate;

public class WebServiceCommunicateContextOptions
{
    public string Url { get; set; }
    public string Namespace{get;set;}
    public string InterfaceName { get; set; }
    public string MethodName { get; set; }
    public string ParameterName { get; set; }
    public Action<string> Logger { get; set; }
}
