namespace Quick.JGST14.ElectronicGate.Model_84
{
    /// <summary>
    /// GPS信息
    /// </summary>
    public class GPS
    {
        ///GPS ID号
        public string GPS_ID { get; set; }
        /// <summary>
        /// 操作：S 开启，C 关闭
        /// </summary>
        public string GPS_OPERATE { get; set; }
        /// <summary>
        /// 结果。0 无操作，1 成功，2 失败
        /// </summary>
        public string RESULT { get; set; }
        /// <summary>
        /// 结果描述。0 无操作，1 成功，2 失败（值可以为空）
        /// </summary>
        public string RESULT_DESCRIPTION { get; set; }
    }
}