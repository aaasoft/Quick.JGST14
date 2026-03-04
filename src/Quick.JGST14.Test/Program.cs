using System.Text;
using System.Xml.Serialization;
using Quick.JGST14.ElectronicGate.Model_81;

var serializer_81 = new XmlSerializer(typeof(GATHER_INFO));
var gatherInfo = new GATHER_INFO()
{
    AREA_ID = "00000000001",
    CHNL_NO = "00000000002",
    CONTA_ID = "00000000003",
    VE_LICENSE_NO = "川X00001",
    DR_RFID = new()
    {
        RFID_ID = "DR0001",
        DR_NAME = "张三"
    }
};
using(var ms = new MemoryStream())
{
    serializer_81.Serialize(ms, gatherInfo);
    var str = Encoding.UTF8.GetString(ms.ToArray());
    Console.WriteLine(str);
}
