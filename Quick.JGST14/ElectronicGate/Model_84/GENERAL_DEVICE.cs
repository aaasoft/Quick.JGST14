namespace Quick.JGST14.ElectronicGate.Model_84
{
    public class GENERAL_DEVICE
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public string DEVICE_TYPE { get; set; }
        /// <summary>
        /// 设备 id
        /// </summary>
        public string DEVIC_ID { get; set; }
        /// <summary>
        /// 操作
        /// </summary>
        public string OPERATE { get; set; }        
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