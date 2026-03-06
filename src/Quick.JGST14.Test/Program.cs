using System.Text;
using System.Xml.Serialization;
using Quick.JGST14.ElectronicGate;
using Quick.JGST14.ElectronicGate.Model_81;

var serializer_81 = new XmlSerializer(typeof(GATHER_INFO));
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
using (var ms = new MemoryStream())
{
    serializer_81.Serialize(ms, gatherInfo);
    var str = Encoding.UTF8.GetString(ms.ToArray());
    Console.WriteLine(str);
}

var tcpCommunicateContext = new TcpCommunicateContext(new()
{
    RemoteHost = "127.0.0.1",
    RemotePort = 5001,
    Logger = Console.WriteLine
});
tcpCommunicateContext.GatherInfoReceived +=(sender,e)=>
{
    Console.WriteLine();
};
tcpCommunicateContext.Start();
//await tcpCommunicateContext.SendGatherInfoAsync(gatherInfo);
Console.ReadLine();