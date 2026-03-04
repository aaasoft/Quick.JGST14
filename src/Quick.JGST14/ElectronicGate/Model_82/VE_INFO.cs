namespace Quick.JGST14.ElectronicGate.Model_82
{
    /// <summary>
    /// 车辆信息
    /// </summary>
    public class VE_INFO
    {
        /// <summary>
        /// 海关车辆编号（值可以为空）
        /// </summary>
        public string VE_CUSTOMS_NO { get; set; }
        /// <summary>
        /// 车牌号（值可以为空）
        /// </summary>
        public string VE_LICENSE_NO { get; set; }
        /// <summary>
        /// 境外或港澳车牌号（值可以为空）
        /// </summary>
        public string VE_LICENSE_NO2 { get; set; }
        /// <summary>
        /// 司机海关编号（值可以为空）
        /// </summary>
        public string DR_CUSTOMS_NO { get; set; }
        /// <summary>
        /// 司机姓名（值可以为空）
        /// </summary>
        public string DR_NAME { get; set; }
        /// <summary>
        /// 托架号（值可以为空）
        /// </summary>
        public string TRAILER_ID { get; set; }
        /// <summary>
        /// 地磅误差（值可以为空）
        /// </summary>
        public string WEIGHT_DIFF { get; set; }
        /// <summary>
        /// 地磅称重（值可以为空）
        /// </summary>
        public string CHANNEL_WEIGHT { get; set; }
        /// <summary>
        /// 车辆理论总重量（值可以为空）
        /// </summary>
        public string GROSS_WEIGHT { get; set; }
        /// <summary>
        /// 车辆自重（值可以为空）
        /// </summary>
        public string VEHICLE_WEIGHT { get; set; }
    }
}