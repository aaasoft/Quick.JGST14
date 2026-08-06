namespace Quick.JGST14.ElectronicGate.Model_83
{
    /// <summary>
    /// GPS信息
    /// </summary>
    public class GPS
    {
        /// <summary>
        /// 车辆海关编号（值可以为空）
        /// </summary>
        public string VE_CUSTOMS_NO { get; set; }
        ///GPS ID号
        public string GPS_ID { get; set; }
        /// <summary>
        /// 启运地海关编码
        /// </summary>
        public string ORIGIN_CUSTOMS { get; set; }
        /// <summary>
        /// 指运地海关编码
        /// </summary>
        public string DEST_CUSTOMS { get; set; }
        /// <summary>
        /// 单证号码，以便查询（值可以为空）
        /// </summary>
        public string FORM_ID { get; set; }
    }
}