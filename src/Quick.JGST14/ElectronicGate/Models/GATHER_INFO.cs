namespace Quick.JGST14.ElectronicGate.Models
{
    /// <summary>
    /// 采集报文
    /// </summary>
    public class GATHER_INFO
    {
        /// <summary>
        /// 进出口标志。I：进口、E：出口
        /// </summary>
        public string I_E_FLAG { get; set; }
        /// <summary>
        /// 场站编号
        /// </summary>
        public string AREA_ID { get; set; }
        /// <summary>
        /// 通道编号
        /// </summary>
        public string CHNL_NO { get; set; }
        /// <summary>
        /// 采集方式。采集方式：A 自动，M 人工补采。B 由后台人工录入验放。默认如果不存在该节点则认 为是自动。
        /// </summary>
        public string GETHER_MODE { get; set; }
        /// <summary>
        /// 会话编号
        /// </summary>
        public string SESSION_ID { get; set; }
        /// <summary>
        /// IC卡信息
        /// </summary>
        public IC_INFO IC_INFO { get; set; }
        /// <summary>
        /// 单证信息
        /// </summary>
        public FORM_INFO FORM_INFO { get; set; }
        /// <summary>
        /// 驾驶员编号（可为空）
        /// </summary>
        public string DR_CUSTOMS_NO { get; set; }
        /// <summary>
        /// 车辆海关编号（可以为空）
        /// </summary>
        public string VE_CUSTOMS_NO { get; set; }
        /// <summary>
        /// 车牌号（可以为空）
        /// </summary>
        public string VE_LICENSE_NO { get; set; }
        /// <summary>
        /// 集装箱号
        /// </summary>
        public string CONTA_ID { get; set; }
        /// <summary>
        /// 安全智能锁号（值可以为空）
        /// </summary>
        public string ESEAL_ID { get; set; }
        /// <summary>
        /// 地磅称重（值可以为空）
        /// </summary>
        public string GROSS_WT { get; set; }
        /// <summary>
        /// 车辆信息
        /// </summary>
        public VE_RFID VE_RFID { get; set; }
        /// <summary>
        /// 驾驶员信息
        /// </summary>
        public DR_RFID DR_RFID { get; set; }
        /*

        */
    }
}