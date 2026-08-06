namespace Quick.JGST14.ElectronicGate.Model_81
{
    /// <summary>
    /// 单证信息
    /// </summary>
    public class FORM_INFO
    {
        /// <summary>
        /// 单证号类型。单证号类型:bill-提单号;entry-报关单号 ;rmft- 公路舱单号 ;cdl- 集报清单 ;oneoff 一次性临时来往粤港小汽车
        /// </summary>
        public string FORM_TYPE { get; set; }
        ///单证号
        public string FORM_ID { get; set; }
    }
}