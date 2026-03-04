namespace Quick.JGST14.ElectronicGate.Model_81
{
    /// <summary>
    /// 箱号识别
    /// </summary>
    public class CONTA_RECOGNITION
    {
        /// <summary>
        /// 箱号
        /// </summary>
        public string CONTA_ID { get; set; }
        /// <summary>
        /// 箱型
        /// </summary>
        public string CONTA_TYPE { get; set; }
        /// <summary>
        /// 箱是否上锁
        /// </summary>
        public string CONTA_LOCK { get; set; }
        /// <summary>
        /// 识别自信度，Y:自信，N:不自信
        /// </summary>
        public string CONFIDENCE_RATIO { get; set; }
    }
}