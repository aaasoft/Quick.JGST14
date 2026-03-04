namespace Quick.JGST14.ElectronicGate.Model_81
{
    /// <summary>
    /// 车牌识别
    /// </summary>
    public class VE_LICENSE_RECOGNITION
    {
        /// <summary>
        /// 国内车牌号
        /// </summary>
        public string DOMESTIC_LICENSE_NO { get; set; }
        /// <summary>
        /// 国内车牌颜色
        /// </summary>
        public string DOMESTIC_LICENSE_COLOR { get; set; }
        /// <summary>
        /// 境外车牌号
        /// </summary>
        public string FOREIGN_LICENSE_NO { get; set; }
        /// <summary>
        /// 境外车牌颜色
        /// </summary>
        public string FOREIGN_LICENSE_COLOR { get; set; }
        /// <summary>
        /// 识别自信度，Y:自信，N:不自信
        /// </summary>
        public string CONFIDENCE_RATIO { get; set; }
    }
}