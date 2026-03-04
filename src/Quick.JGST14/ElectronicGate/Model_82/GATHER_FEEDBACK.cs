namespace Quick.JGST14.ElectronicGate.Model_82
{
    /// <summary>
    /// 采集反馈
    /// </summary>
    public class GATHER_FEEDBACK
    {
        /// <summary>
        /// 场站编号
        /// </summary>
        public string AREA_ID { get; set; }
        /// <summary>
        /// 通道编号
        /// </summary>
        public string CHNL_NO { get; set; }
        /// <summary>
        /// 会话编号
        /// </summary>
        public string SESSION_ID { get; set; }
        /// <summary>
        /// 验放列表类型
        /// </summary>
        public string RELLIST_TYPE { get; set; }
        /// <summary>
        /// 验放列表ID类型
        /// </summary>
        public string RELLIST_ID_TYPE { get; set; }
        /// <summary>
        /// 验放列表ID
        /// </summary>
        public string RELLIST_ID { get; set; }
        /// <summary>
        /// 后台核放时间
        /// </summary>
        public string FEEDBACK_TIME { get; set; }
        /// <summary>
        /// 最终处理结果：Y 为放行，N 为报警，M 为待人工确认
        /// </summary>
        public string CHECK_RESULT { get; set; }
        /// <summary>
        /// 设备操作指令。后台对前端的敏感设备操作
        /// </summary>
        public string INSTRUCTION { get; set; }
        /// <summary>
        /// 业务逻辑错误码。海关业务方面对返回结果代码，返回给关员
        /// </summary>
        public string PROC_ERROR_CODE { get; set; }
        /// <summary>
        /// 业务逻辑错误解释。海关业务方面对返回结果的解释，返回给关员
        /// </summary>
        public string PROC_ERROR_DESCRIPTION { get; set; }
        /// <summary>
        /// 技术错误码。后台及网络方面的故障代码
        /// </summary>
        public string TECH_ERROR_CODE { get; set; }
        /// <summary>
        /// 技术错误解释。后台及网络方面的故障解释
        /// </summary>
        public string TECH_ERROR_DESCRIPTION { get; set; }
        /// <summary>
        /// IC卡信息
        /// </summary>
        public IC_INFO IC_INFO { get; set; }
        /// <summary>
        /// 车辆信息
        /// </summary>
        public VE_INFO VE_INFO { get; set; }
        /// <summary>
        /// 集装箱号（值可以为空）
        /// </summary>
        public string CONTA_ID { get; set; }
        /// <summary>
        /// 安全智能锁号（值可以为空）
        /// </summary>
        public string ESEAL_ID { get; set; }
        /// <summary>
        /// 铅封号（可以为空）
        /// </summary>
        public string SEAL_ID { get; set; }
        /// <summary>
        /// 单证信息
        /// </summary>
        public FORM_INFO FORM_INFO { get; set; }
        /// <summary>
        /// 单证总件数（可以为空）
        /// </summary>
        public string PACK_NO { get; set; }
        /// <summary>
        /// 舱单总件数（可以为空）
        /// </summary>
        public string DECL_PACK { get; set; }
        /// <summary>
        /// 舱单货物总重量（可以为空）
        /// </summary>
        public string DECL_GOODS_WEIGHT { get; set; }
        /// <summary>
        /// 核放结果详细信息（布控、地磅等）
        /// </summary>
        public string OP_HINT { get; set; }
        /// <summary>
        /// LED操作提示
        /// </summary>
        public string LED_HINT { get; set; }
        /// <summary>
        /// 扩展内容
        /// </summary>
        public string EXTENDED_CONTENT { get; set; }
    }
}