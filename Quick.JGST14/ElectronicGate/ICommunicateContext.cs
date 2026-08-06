namespace Quick.JGST14.ElectronicGate;

public interface ICommunicateContext
{
    event EventHandler<ModelInfo<Model_81.GATHER_INFO>> GatherInfoReceived;
    event EventHandler<ModelInfo<Model_82.GATHER_FEEDBACK>> GatherFeedbackReceived;
    event EventHandler<ModelInfo<Model_83.OPERATE_INFO>> OperateInfoReceived;
    event EventHandler<ModelInfo<Model_84.OPERATE_FEEDBACK>> OperateFeedbackReceived;
    event EventHandler<ModelInfo<Model_85.MANUAL_CHECK>> ManualCheckReceived;

    void Start();
    void Stop();
    Task SendGatherInfoAsync(Model_81.GATHER_INFO model, CancellationToken cancellationToken = default);
    Task SendGatherFeedbackAsync(Model_82.GATHER_FEEDBACK model, string ieFlag, CancellationToken cancellationToken = default);
    Task SendOperateInfoAsync(Model_83.OPERATE_INFO model, string ieFlag, CancellationToken cancellationToken = default);
    Task SendOperateFeedbackAsync(Model_84.OPERATE_FEEDBACK model, string ieFlag, CancellationToken cancellationToken = default);
    Task SendManualCheckAsync(Model_85.MANUAL_CHECK model, string ieFlag, CancellationToken cancellationToken = default);
    Task SendAsync(string xml, DataType dataType, string areaId, string channelId, string ieFlag, CancellationToken cancellationToken = default);
    Task SendAsync<T>(T model, string ieFlag, CancellationToken cancellationToken = default) where T : IModel;
}