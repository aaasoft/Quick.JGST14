namespace Quick.JGST14.ElectronicGate.Model_84
{
    /// <summary>
    /// 安全智能锁信息
    /// </summary>
    public class ESEAL
    {
        /// <summary>
        /// 安全智能锁号码。多锁以”|”分隔
        /// </summary>
        public string ESEAL_ID { get; set; }
        /// <summary>
        /// 操作：U 开锁，L 加锁
        /// </summary>
        public string ESEAL_OPERATE { get; set; }
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