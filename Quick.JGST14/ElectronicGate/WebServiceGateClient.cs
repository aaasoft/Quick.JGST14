using System.Text;
using System.Xml.Linq;

namespace Quick.JGST14.ElectronicGate;

/// <summary>
/// WebService卡口客户端
/// </summary>
public class WebServiceGateClient : AbstractGateClient
{
    private WebServiceGateClientOptions options;
    public WebServiceGateClient(WebServiceGateClientOptions options)
    {
        this.options = options;
    }

    public override async Task SendAsync(string xml, CancellationToken cancellationToken = default)
    {
        // 构造SOAP请求体
        string soapBody = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <{options.MethodName} xmlns=""{options.Namespace}"">
      <{options.ParameterName}>{System.Security.SecurityElement.Escape(xml)}</{options.ParameterName}>
    </{options.MethodName}>
  </soap:Body>
</soap:Envelope>";

        using var httpClient = new HttpClient();
        var content = new StringContent(soapBody, Encoding.UTF8, "text/xml");
        // SOAPAction头很关键，大部分webservice必须要
        httpClient.DefaultRequestHeaders.Add("SOAPAction", $"{options.Namespace}/{options.InterfaceName}/{options.MethodName}");

        HttpResponseMessage response = await httpClient.PostAsync(options.Url, content);
        string responseXml = await response.Content.ReadAsStringAsync();

        options.Logger?.Invoke("服务返回原始xml：");
        options.Logger?.Invoke(responseXml);

        if (!response.IsSuccessStatusCode)
            return;

        _ = Task.Run(() =>
        {
            // 解析返回XML拿到结果
            XDocument doc = XDocument.Parse(responseXml);
            XNamespace ns = options.Namespace;
            var resultNode = doc.Descendants(ns + $"{options.MethodName}Result");
            if (resultNode != null)
            {
                string data = resultNode.FirstOrDefault()?.Value;
                var model = Deserialize(data);
                if (model is Model_81.GATHER_INFO model81)
                    RaiseEvent_GatherInfoReceived(model81);
                else if (model is Model_82.GATHER_FEEDBACK model82)
                    RaiseEvent_GatherFeedbackReceived(model82);
                else if (model is Model_83.OPERATE_INFO model83)
                    RaiseEvent_OperateInfoReceived(model83);
                else if (model is Model_84.OPERATE_FEEDBACK model84)
                    RaiseEvent_OperateFeedbackReceived(model84);
                else if (model is Model_85.MANUAL_CHECK model85)
                    RaiseEvent_ManualCheckReceived(model85);
            }
        });
    }

    public override Task SendAsync<T>(T model, CancellationToken cancellationToken = default)
    {
        var xml = Serialize(model);
        return SendAsync(xml, cancellationToken);
    }
}