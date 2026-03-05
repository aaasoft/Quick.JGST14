using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Quick.JGST14.Utils;

namespace Quick.JGST14.ElectronicGate
{
    /// <summary>
    /// TCP通讯上下文
    /// </summary>
    public class TcpCommunicateContext
    {
        /// <summary>
        /// 包头
        /// </summary>
        public class PacketHead
        {
            public const int Size = 38;
            public PacketHead(Memory<byte> buffer)
            {
                var currentMemory = buffer;

                Head = currentMemory.Slice(0, 4);
                currentMemory = currentMemory.Slice(4);

                TotalLength = currentMemory.Slice(0, 4);
                currentMemory = currentMemory.Slice(4);

                DataCode = currentMemory.Slice(0, 1);
                currentMemory = currentMemory.Slice(1);

                AreaId = currentMemory.Slice(0, 10);
                currentMemory = currentMemory.Slice(10);

                ChannelId = currentMemory.Slice(0, 10);
                currentMemory = currentMemory.Slice(10);

                IeTag = currentMemory.Slice(0, 1);
                currentMemory = currentMemory.Slice(1);

                BiaoZhiFu = currentMemory.Slice(0, 4);
                currentMemory = currentMemory.Slice(4);

                XmlStreamLength = currentMemory.Slice(0, 4);
            }

            /// <summary>
            /// 包头标记
            /// </summary>
            public Memory<byte> Head;
            /// <summary>
            /// 总长
            /// </summary>
            public Memory<byte> TotalLength;
            /// <summary>
            /// 报文代码
            /// </summary>
            public Memory<byte> DataCode;
            /// <summary>
            /// 场站号
            /// </summary>
            public Memory<byte> AreaId;
            /// <summary>
            /// 通道号
            /// </summary>
            public Memory<byte> ChannelId;
            /// <summary>
            /// 进出标志
            /// </summary>
            public Memory<byte> IeTag;
            /// <summary>
            /// 标识符
            /// </summary>            
            public Memory<byte> BiaoZhiFu;
            /// <summary>
            /// XML流长度
            /// </summary>
            public Memory<byte> XmlStreamLength;
        }

        /// <summary>
        /// 包头标记
        /// </summary>
        private static readonly byte[] packetStartHead = [0xE2, 0x5C, 0x4B, 0x89];
        /// <summary>
        /// 标志符
        /// </summary>
        private static byte[] packetTagHead = [0xFF, 0xFF, 0xFF, 0xFF];
        /// <summary>
        /// 包尾标记
        /// </summary>
        private static byte[] packetEndTail = [0xFF, 0xFF];
        private byte[] sendBuffer;
        private byte[] recvBuffer;

        private TcpCommunicateContextOptions options;
        private CancellationTokenSource cts;
        private TcpListener tcpListener;

        public event EventHandler<Model_81.GATHER_INFO> GatherInfoReceived;
        public event EventHandler<Model_82.GATHER_FEEDBACK> GatherFeedbackReceived;
        public event EventHandler<Model_83.OPERATE_INFO> OperateInfoReceived;
        public event EventHandler<Model_84.OPERATE_FEEDBACK> OperateFeedbackReceived;
        public event EventHandler<Model_85.MANUAL_CHECK> ManualCheckReceived;

        public TcpCommunicateContext(TcpCommunicateContextOptions options)
        {
            this.options = options;
            sendBuffer = new byte[options.SendBufferSize];
            recvBuffer = new byte[options.RecvBufferSize];
        }


        public void Start()
        {
            Stop();
            cts = new CancellationTokenSource();
            tcpListener = new TcpListener(IPAddress.Parse(options.LocalListenIpAddress), options.LocalListenPort);
            tcpListener.Start();
            options.Logger?.Invoke($"已经开始监听[{options.LocalListenIpAddress}:{options.LocalListenPort}]。");
            _ = beginAcceptClient(tcpListener, cts.Token);
        }

        private async Task beginAcceptClient(TcpListener tcpListener, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();
                _ = beginReadClient(tcpClient, cancellationToken);
            }
        }

        private bool isBufferMatchHeader(Span<byte> buffer, Span<byte> header)
        {
            if (buffer == null || header == null)
                return false;
            if (buffer.Length != header.Length)
                return false;
            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != header[i])
                    return false;
            }
            return true;
        }

        private string ToDecimalString(Span<byte> span)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < span.Length; i++)
                sb.Append(span[i].ToString());
            return sb.ToString();
        }

        private byte[] FromDecimalString(string text)
        {
            var array = new byte[text.Length];
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                array[i] = byte.Parse(c.ToString());
            }
            return array;
        }

        private int Serialize<T>(T t, byte[] buffer, int start, int length)
        {
            using (var ms = new MemoryStream(buffer, start, length))
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(ms, t);
                return (int)ms.Position;
            }
        }

        private T Deserialize<T>(byte[] buffer, int start, int length)
        {
            using (var ms = new MemoryStream(buffer, start, length))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(ms);
            }
        }

        private async Task beginReadFromStream(Stream stream, CancellationToken cancellationToken)
        {
            int index = 0;
            var freeRecvMemory = new Memory<byte>(recvBuffer);
            while (!cancellationToken.IsCancellationRequested)
            {
                //读取包头
                var packetHeadMemory = freeRecvMemory.Slice(0, PacketHead.Size);
                await stream.ReadExactlyAsync(packetHeadMemory);
                freeRecvMemory = freeRecvMemory.Slice(PacketHead.Size);
                index += PacketHead.Size;

                var packetHead = new PacketHead(packetHeadMemory);
                //检查包头标记是否匹配
                if (!isBufferMatchHeader(packetHead.Head.Span, packetStartHead))
                {
                    throw new IOException($"接收到的包头标记[{BitConverter.ToString(packetHead.Head.ToArray())}]不正确。");
                }
                //读取包长度
                var packetTotalLength = ByteUtils.B2I_BE(packetHead.TotalLength);
                //读取报文代码
                var modelType = (ModelType)packetHead.DataCode.Span[0];
                //读取场站号
                var areaId = ToDecimalString(packetHead.AreaId.Span);
                //读取通道号
                var channelId = ToDecimalString(packetHead.ChannelId.Span);
                //读取进出标志
                var ieFlag = char.ConvertFromUtf32(packetHead.IeTag.Span[0]);
                //检查标志符是否匹配
                if (!isBufferMatchHeader(packetHead.BiaoZhiFu.Span, packetTagHead))
                {
                    throw new IOException($"接收到的标志符[{BitConverter.ToString(packetHead.BiaoZhiFu.ToArray())}]不正确。");
                }
                //读取XML流长度
                var xmlTotalLength = ByteUtils.B2I_BE(packetHead.XmlStreamLength);
                //读取XML格式数据
                var xmlContentMemory = freeRecvMemory.Slice(0, xmlTotalLength);
                await stream.ReadExactlyAsync(xmlContentMemory, cancellationToken);
                switch (modelType)
                {
                    case ModelType.GatherInfo:
                        GatherInfoReceived?.Invoke(this, Deserialize<Model_81.GATHER_INFO>(recvBuffer, index, xmlTotalLength));
                        break;
                    case ModelType.GatherFeedback:
                        GatherFeedbackReceived?.Invoke(this, Deserialize<Model_82.GATHER_FEEDBACK>(recvBuffer, index, xmlTotalLength));
                        break;
                    case ModelType.OperateInfo:
                        OperateInfoReceived?.Invoke(this, Deserialize<Model_83.OPERATE_INFO>(recvBuffer, index, xmlTotalLength));
                        break;
                    case ModelType.OperateFeedback:
                        OperateFeedbackReceived?.Invoke(this, Deserialize<Model_84.OPERATE_FEEDBACK>(recvBuffer, index, xmlTotalLength));
                        break;
                    case ModelType.ManualCheck:
                        ManualCheckReceived?.Invoke(this, Deserialize<Model_85.MANUAL_CHECK>(recvBuffer, index, xmlTotalLength));
                        break;
                }
                index += xmlTotalLength;
                //读取包尾标记
                var headTailMemory = freeRecvMemory.Slice(0, 2);
                await stream.ReadExactlyAsync(headTailMemory, cancellationToken);
                //检查包尾标记是否匹配
                if (!isBufferMatchHeader(headTailMemory.Span, packetEndTail))
                {
                    throw new IOException($"接收到的包尾标记[{BitConverter.ToString(recvBuffer, index, headTailMemory.Length)}]不正确。");
                }
                index += headTailMemory.Length;
            }
        }

        private async Task beginReadClient(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            options.Logger?.Invoke($"[{tcpClient.Client.RemoteEndPoint}]已经连接，开始从接收数据。。。");
            try
            {
                using (var stream = tcpClient.GetStream())
                    await beginReadFromStream(stream, cancellationToken);
            }
            catch (Exception ex)
            {
                options.Logger?.Invoke($"从[{tcpClient.Client.RemoteEndPoint}]接收解析数据时出错，原因：{ex}");
                tcpClient.Dispose();
            }
        }

        public void Stop()
        {
            cts?.Cancel();
            cts = null;
            tcpListener?.Stop();
            tcpListener = null;
        }

        public async Task SendGatherInfoAsync(Model_81.GATHER_INFO model, CancellationToken cancellationToken = default)
        {
            await SendAsync(model.AREA_ID, model.CHNL_NO, ModelType.GatherInfo, model.I_E_FLAG, model, cancellationToken);
        }

        public async Task SendGatherFeedbackAsync(Model_82.GATHER_FEEDBACK model, string ieFlag, CancellationToken cancellationToken = default)
        {
            await SendAsync(model.AREA_ID, model.CHNL_NO, ModelType.GatherFeedback, ieFlag, model, cancellationToken);
        }

        public async Task SendOperateInfoAsync(Model_83.OPERATE_INFO model, string ieFlag, CancellationToken cancellationToken = default)
        {
            await SendAsync(model.AREA_ID, model.CHNL_NO, ModelType.OperateInfo, ieFlag, model, cancellationToken);
        }

        public async Task SendOperateFeedbackAsync(Model_84.OPERATE_FEEDBACK model, string ieFlag, CancellationToken cancellationToken = default)
        {
            await SendAsync(model.AREA_ID, model.CHNL_NO, ModelType.OperateFeedback, ieFlag, model, cancellationToken);
        }

        public async Task SendManualCheckAsync(Model_85.MANUAL_CHECK model, string ieFlag, CancellationToken cancellationToken = default)
        {
            await SendAsync(model.AREA_ID, model.CHNL_NO, ModelType.ManualCheck, ieFlag, model, cancellationToken);
        }

        private async Task SendAsync<T>(string areaId, string channelId, ModelType modelType, string ieTag, T model, CancellationToken cancellationToken = default)
        {            
            //写入XML流内容
            var xmlStreamLengthNumber = Serialize(model, sendBuffer, PacketHead.Size, sendBuffer.Length - PacketHead.Size);

            //写入包头
            var xmlStreamLength = BitConverter.GetBytes(xmlStreamLengthNumber);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(xmlStreamLength);

            var totalLengthNumber = PacketHead.Size + xmlStreamLengthNumber + packetEndTail.Length;
            var totalLength = BitConverter.GetBytes(totalLengthNumber);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(totalLength);

            var packetHead = new PacketHead(sendBuffer);
            packetStartHead.CopyTo(packetHead.Head);
            packetTagHead.CopyTo(packetHead.BiaoZhiFu);
            FromDecimalString(areaId).CopyTo(packetHead.AreaId);
            FromDecimalString(channelId).CopyTo(packetHead.ChannelId);
            packetHead.DataCode.Span[0] = (byte)modelType;
            packetHead.IeTag.Span[0] =Encoding.Default.GetBytes(ieTag)[0];
            xmlStreamLength.CopyTo(packetHead.XmlStreamLength);
            totalLength.CopyTo(packetHead.TotalLength);

            //写入包尾
            packetEndTail.CopyTo(sendBuffer, PacketHead.Size + xmlStreamLengthNumber);

            using (var tcpClient = new TcpClient())
            {
                //连接
                await tcpClient.ConnectAsync(IPAddress.Parse(options.RemoteHost), options.RemotePort, cancellationToken);
                using (var ns = tcpClient.GetStream())
                {
                    await ns.WriteAsync(sendBuffer, 0, totalLengthNumber, cancellationToken);
                    await ns.FlushAsync();
                }
                options.Logger?.Invoke($"已向[{options.RemoteHost}:{options.RemotePort}]发送数据。数据大小：{totalLengthNumber}，数据内容：{BitConverter.ToString(sendBuffer, 0, totalLengthNumber)}");
                tcpClient.Close();
                tcpClient.Dispose();
            }
        }
    }
}