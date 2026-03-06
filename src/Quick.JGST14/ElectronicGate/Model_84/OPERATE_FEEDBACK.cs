namespace Quick.JGST14.ElectronicGate.Model_84
{
    /// <summary>
    /// 设备操作
    /// </summary>
    public class OPERATE_FEEDBACK : IModel
    {
        public DataType GetDataType() => DataType.OperateFeedback;
        /// <summary>
        /// 场站编号
        /// </summary>
        public string AREA_ID { get; set; }
        /// <summary>
        /// 通道编号
        /// </summary>
        public string CHNL_NO { get; set; }
        /// <summary>
        /// 会话编号
        /// </summary>
        public string SESSION_ID { get; set; }
        /// <summary>
        /// GPS信息
        /// </summary>
        public GPS GPS { get; set; }
        /// <summary>
        /// 安全智能锁信息
        /// </summary>
        public ESEAL ESEAL { get; set; }
        public GENERAL_DEVICE GENERAL_DEVICE { get; set; }
    }
}