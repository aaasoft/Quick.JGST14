namespace Quick.JGST14.ElectronicGate.Models
{
    /// <summary>
    /// 驾驶员信息
    /// </summary>
    public class DR_RFID
    {
        /// <summary>
        /// 电子司机卡号
        /// </summary>
        public string RFID_ID { get; set; }
        /// <summary>
        /// 司机姓名（值可以为空）
        /// </summary>
        public string DR_NAME { get; set; }
        /// <summary>
        /// 司机海关编号（值可以为空）
        /// </summary>
        public string DR_CUSTOMS_NO { get; set; }
        /// <summary>
        /// 司机所属公司（值可以为空）
        /// </summary>
        public string DR_COMPANY { get; set; }
        /// <summary>
        /// 单位时间读卡次数（值可以为空）
        /// </summary>
        public string DR_PERFORMANCE { get; set; }
    }
}