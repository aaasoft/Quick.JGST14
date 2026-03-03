using System;

namespace Quick.JGST14.ElectronicGate.Models
{
    /// <summary>
    /// 车辆信息
    /// </summary>
    public class VE_RFID
    {
        /// <summary>
        /// 电子车牌 ID
        /// </summary>
        public string RFID_ID { get; set; }
        /// <summary>
        /// 车牌号（值可以为空）
        /// </summary>
        public string VE_LICENSE_NO { get; set; }
        /// <summary>
        /// 海关车辆编号（值可以为空）
        /// </summary>
        public string VE_CUSTOMS_NO { get; set; }
        /// <summary>
        /// 车自重（值可以为空）
        /// </summary>
        public string VE_WT { get; set; }
        /// <summary>
        /// 车辆所属公司（值可以为空）
        /// </summary>
        public string VE_COMPANY { get; set; }
        /// <summary>
        /// 单位时间读卡次数（值可以为空）
        /// </summary>
        public string VE_PERFORMANCE { get; set; }
    }
}