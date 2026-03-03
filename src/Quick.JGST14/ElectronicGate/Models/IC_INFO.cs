namespace Quick.JGST14.ElectronicGate.Models
{
    /// <summary>
    /// IC卡信息
    /// </summary>
    public class IC_INFO
    {
        /// <summary>
        /// 32 位全球唯一编码
        /// </summary>
        public string IC_ID { get; set; }
        /// <summary>
        /// IC 卡的编号
        /// </summary>
        public string IC_NO { get; set; }
        /// <summary>
        /// IC 卡类型
        /// </summary>
        public string IC_TYPE { get; set; }
        /// <summary>
        /// 结构体。可扩展 IC 卡内容，结构体,散杂货等业务可使用。
        /// </summary>
        public string IC_EXTENDED_CONTENT { get; set; }
    }
}