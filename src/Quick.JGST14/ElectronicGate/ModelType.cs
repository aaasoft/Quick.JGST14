namespace Quick.JGST14.ElectronicGate
{
    public enum ModelType : byte
    {
        /// <summary>
        /// 采集报文
        /// </summary>
        GatherInfo = 0x81,
        /// <summary>
        /// 采集反馈
        /// </summary>
        GatherFeedback = 0x82,
        /// <summary>
        /// 设备操作指令
        /// </summary>
        OperateInfo = 0x83,
        /// <summary>
        /// 设备操作反馈
        /// </summary>
        OperateFeedback = 0x84,
        /// <summary>
        /// 人工确认报文
        /// </summary>
        ManualCheck = 0x85,
    }
}