namespace Quick.JGST14.ElectronicGate.Model_85
{
    /// <summary>
    /// 人工确认
    /// </summary>
    public class MANUAL_CHECK : IModel
    {
        public DataType GetDataType() => DataType.ManualCheck;
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
        /// 确认结果:1 放行,2 报警转查验,3 不放行,退回
        /// </summary>
        public string CHECK_RESULT { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public string COMMENT { get; set; }
        /// <summary>
        /// 审核关员
        /// </summary>
        public string OPERATOR_ID { get; set; }
    }
}