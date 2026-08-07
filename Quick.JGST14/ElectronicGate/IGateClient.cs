namespace Quick.JGST14.ElectronicGate;

public interface IGateClient
{
    event EventHandler<Model_81.GATHER_INFO> GatherInfoReceived;
    event EventHandler<Model_82.GATHER_FEEDBACK> GatherFeedbackReceived;
    event EventHandler<Model_83.OPERATE_INFO> OperateInfoReceived;
    event EventHandler<Model_84.OPERATE_FEEDBACK> OperateFeedbackReceived;
    event EventHandler<Model_85.MANUAL_CHECK> ManualCheckReceived;

    void Start();
    void Stop();
    string Serialize(IModel model);
    IModel Deserialize(string xml);
    Task SendGatherInfoAsync(Model_81.GATHER_INFO model, CancellationToken cancellationToken = default);
    Task SendGatherFeedbackAsync(Model_82.GATHER_FEEDBACK model, CancellationToken cancellationToken = default);
    Task SendOperateInfoAsync(Model_83.OPERATE_INFO model, CancellationToken cancellationToken = default);
    Task SendOperateFeedbackAsync(Model_84.OPERATE_FEEDBACK model, CancellationToken cancellationToken = default);
    Task SendManualCheckAsync(Model_85.MANUAL_CHECK model, CancellationToken cancellationToken = default);
    Task SendAsync(string xml, CancellationToken cancellationToken = default);
    Task SendAsync<T>(T model, CancellationToken cancellationToken = default) where T : IModel;
}