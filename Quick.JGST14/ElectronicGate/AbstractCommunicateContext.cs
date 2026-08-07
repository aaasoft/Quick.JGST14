using System.Xml.Linq;
using System.Xml.Serialization;

namespace Quick.JGST14.ElectronicGate;

public abstract class AbstractCommunicateContext : ICommunicateContext
{
    public event EventHandler<Model_81.GATHER_INFO> GatherInfoReceived;
    public event EventHandler<Model_82.GATHER_FEEDBACK> GatherFeedbackReceived;
    public event EventHandler<Model_83.OPERATE_INFO> OperateInfoReceived;
    public event EventHandler<Model_84.OPERATE_FEEDBACK> OperateFeedbackReceived;
    public event EventHandler<Model_85.MANUAL_CHECK> ManualCheckReceived;

    protected void RaiseEvent_GatherInfoReceived(Model_81.GATHER_INFO model) => GatherInfoReceived?.Invoke(this, model);
    protected void RaiseEvent_GatherFeedbackReceived(Model_82.GATHER_FEEDBACK model) => GatherFeedbackReceived?.Invoke(this, model);
    protected void RaiseEvent_OperateInfoReceived(Model_83.OPERATE_INFO model) => OperateInfoReceived?.Invoke(this, model);
    protected void RaiseEvent_OperateFeedbackReceived(Model_84.OPERATE_FEEDBACK model) => OperateFeedbackReceived?.Invoke(this, model);
    protected void RaiseEvent_ManualCheckReceived(Model_85.MANUAL_CHECK model) => ManualCheckReceived?.Invoke(this, model);

    public abstract Task SendAsync(string xml, CancellationToken cancellationToken = default);
    public abstract Task SendAsync<T>(T model, CancellationToken cancellationToken = default) where T : IModel;

    public async Task SendGatherInfoAsync(Model_81.GATHER_INFO model, CancellationToken cancellationToken = default)
    {
        await SendAsync(model, cancellationToken);
    }

    public async Task SendGatherFeedbackAsync(Model_82.GATHER_FEEDBACK model, CancellationToken cancellationToken = default)
    {
        await SendAsync(model, cancellationToken);
    }

    public async Task SendOperateInfoAsync(Model_83.OPERATE_INFO model, CancellationToken cancellationToken = default)
    {
        await SendAsync(model, cancellationToken);
    }

    public async Task SendOperateFeedbackAsync(Model_84.OPERATE_FEEDBACK model, CancellationToken cancellationToken = default)
    {
        await SendAsync(model, cancellationToken);
    }

    public async Task SendManualCheckAsync(Model_85.MANUAL_CHECK model, CancellationToken cancellationToken = default)
    {
        await SendAsync(model, cancellationToken);
    }

    private static Dictionary<string, Type> modelTypeDict;
    static AbstractCommunicateContext()
    {
        modelTypeDict = new Dictionary<string, Type>()
        {
            [nameof(Model_81.GATHER_INFO)] = typeof(Model_81.GATHER_INFO),
            [nameof(Model_82.GATHER_FEEDBACK)] = typeof(Model_82.GATHER_FEEDBACK),
            [nameof(Model_83.OPERATE_INFO)] = typeof(Model_83.OPERATE_INFO),
            [nameof(Model_84.OPERATE_FEEDBACK)] = typeof(Model_84.OPERATE_FEEDBACK),
            [nameof(Model_85.MANUAL_CHECK)] = typeof(Model_85.MANUAL_CHECK)
        };
    }

    public virtual void Start() { }

    public virtual void Stop() { }

    public string Serialize(IModel model)
    {
        var xmlSerializer = new XmlSerializer(model.GetType());
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, model);
        return stringWriter.ToString();
    }

    public IModel Deserialize(string xml)
    {
        var document = XDocument.Parse(xml);
        if (!modelTypeDict.TryGetValue(document.Root.Name.LocalName, out var modelType))
            return default;
        var xmlSerializer = new XmlSerializer(modelType);
        using var xmlReader = new StringReader(xml);
        var model = (IModel)xmlSerializer.Deserialize(xmlReader);
        return model;
    }
}