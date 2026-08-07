using System.Text;
using System.Xml.Serialization;
using Quick.JGST14.ElectronicGate;
using Quick.JGST14.ElectronicGate.Model_81;

var gatherInfo = new GATHER_INFO()
{
    AREA_ID = "1000000001",
    CHNL_NO = "2000000002",
    CONTA_ID = "3000000003",
    VE_LICENSE_NO = "川X00001",
    I_E_FLAG = "I",
    DR_RFID = new()
    {
        RFID_ID = "DR0001",
        DR_NAME = "张三"
    }
};
IGateClient communicateContext;
var testType = "WebService";
switch (testType)
{
    case "TCP":
        communicateContext = new TcpGateClient(new()
        {
            RemoteHost = "127.0.0.1",
            RemotePort = 5001,
            Logger = Console.WriteLine
        });
        break;
    case "WebService":
        communicateContext = new WebServiceGateClient(new()
        {
            Url = "http://127.0.0.1:9002/IMessageService",
            Namespace = "http://ws.servicebus.cc",
            InterfaceName = "IRelService",
            MethodName = "RelMessage",
            ParameterName = "message",
            Logger = Console.WriteLine
        });
        break;
    default:
        throw new NotImplementedException();
}

Console.WriteLine(communicateContext.Serialize(gatherInfo));

communicateContext.GatherInfoReceived += (sender, e) =>
{
    Console.WriteLine($"接收到消息：{e.GetDataType()}");
};
communicateContext.GatherFeedbackReceived += (sender, e) =>
{
    Console.WriteLine($"接收到消息：{e.GetDataType()}");
};
communicateContext.Start();
await communicateContext.SendGatherInfoAsync(gatherInfo);
Console.ReadLine();