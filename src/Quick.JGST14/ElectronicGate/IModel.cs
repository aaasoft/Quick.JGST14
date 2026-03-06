namespace Quick.JGST14.ElectronicGate;

public interface IModel
{
    /// <summary>
    /// 场站编号
    /// </summary>
    string AREA_ID { get; set; }
    /// <summary>
    /// 通道编号
    /// </summary>
    string CHNL_NO { get; set; }
    /// <summary>
    /// 会话编号
    /// </summary>
    string SESSION_ID { get; set; }
    /// <summary>
    /// 获取数据类型
    /// </summary>
    /// <returns></returns>
    DataType GetDataType();
}
