namespace Quick.JGST14.ElectronicGate.Model_83
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
        /// 安全智能锁密钥
        /// </summary>
        public string ESEAL_KEY { get; set; }
    }
}