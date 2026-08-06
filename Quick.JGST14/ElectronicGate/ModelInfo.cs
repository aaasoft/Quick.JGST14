namespace Quick.JGST14.ElectronicGate;

/// <summary>
/// 报文信息
/// </summary>
/// <typeparam name="T"></typeparam>
public class ModelInfo<T> where T : IModel
{
    /// <summary>
    /// 报文代码
    /// </summary>
    public DataType DataType { get; set; }
    /// <summary>
    /// 场站号
    /// </summary>
    public string AreaId { get; set; }
    /// <summary>
    /// 通道号
    /// </summary>
    public string ChannelId { get; set; }
    /// <summary>
    /// 进出标志
    /// </summary>
    public string IeFlag { get; set; }
    /// <summary>
    /// 报文
    /// </summary>
    public T Data { get; set; }
}
