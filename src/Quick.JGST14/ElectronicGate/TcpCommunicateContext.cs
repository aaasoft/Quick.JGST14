using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Quick.JGST14.ElectronicGate
{
    /// <summary>
    /// TCP通讯上下文
    /// </summary>
    public class TcpCommunicateContext
    {
        private byte[] sendBuffer;
        private byte[] recvBuffer;

        private TcpCommunicateContextOptions options;
        private CancellationTokenSource cts;
        private TcpListener tcpListener;

        public event EventHandler<ModelInfo<Model_81.GATHER_INFO>> GatherInfoReceived;
        public event EventHandler<ModelInfo<Model_82.GATHER_FEEDBACK>> GatherFeedbackReceived;
        public event EventHandler<ModelInfo<Model_83.OPERATE_INFO>> OperateInfoReceived;
        public event EventHandler<ModelInfo<Model_84.OPERATE_FEEDBACK>> OperateFeedbackReceived;
        public event EventHandler<ModelInfo<Model_85.MANUAL_CHECK>> ManualCheckReceived;

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

        private async Task beginReadFromStream(Stream stream, CancellationToken cancellationToken)
        {
            var freeRecvMemory = new Memory<byte>(recvBuffer);
            while (!cancellationToken.IsCancellationRequested)
            {
                //读取简易包头
                var slimHeadMemory = freeRecvMemory.Slice(0, TcpCommunicatePacket.SLIM_HEAD_SIZE);
                await stream.ReadExactlyAsync(slimHeadMemory);
                var totalLength = TcpCommunicatePacket.ParseTotalLength(slimHeadMemory.Span);
                //读取包剩余部分
                await stream.ReadExactlyAsync(recvBuffer, TcpCommunicatePacket.SLIM_HEAD_SIZE, totalLength - TcpCommunicatePacket.SLIM_HEAD_SIZE);
                //解析包
                var packet = new TcpCommunicatePacket(recvBuffer, true);
                switch (packet.DataType)
                {
                    case DataType.GatherInfo:
                        GatherInfoReceived?.Invoke(this, packet.GetModelInfo<Model_81.GATHER_INFO>());
                        break;
                    case DataType.GatherFeedback:
                        GatherFeedbackReceived?.Invoke(this, packet.GetModelInfo<Model_82.GATHER_FEEDBACK>());
                        break;
                    case DataType.OperateInfo:
                        OperateInfoReceived?.Invoke(this, packet.GetModelInfo<Model_83.OPERATE_INFO>());
                        break;
                    case DataType.OperateFeedback:
                        OperateFeedbackReceived?.Invoke(this, packet.GetModelInfo<Model_84.OPERATE_FEEDBACK>());
                        break;
                    case DataType.ManualCheck:
                        ManualCheckReceived?.Invoke(this, packet.GetModelInfo<Model_85.MANUAL_CHECK>());
                        break;
                }
            }
        }


        private async Task beginReadClient(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            var remoteEndPoint = tcpClient.Client.RemoteEndPoint.ToString();
            options.Logger?.Invoke($"[{remoteEndPoint}]已经连接，开始从接收数据。。。");
            try
            {
                using (var stream = tcpClient.GetStream())
                    await beginReadFromStream(stream, cancellationToken);
            }
            catch (Exception ex)
            {
                tcpClient.Dispose();
                options.Logger?.Invoke($"[{remoteEndPoint}]的连接已经断开。接收解析数据时出错，原因：{ex.Message}");
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
            await SendAsync(model, model.I_E_FLAG, cancellationToken);
        }

        public async Task SendGatherFeedbackAsync(Model_82.GATHER_FEEDBACK model, string ieFlag, CancellationToken cancellationToken = default)
        {
            await SendAsync(model, ieFlag, cancellationToken);
        }

        public async Task SendOperateInfoAsync(Model_83.OPERATE_INFO model, string ieFlag, CancellationToken cancellationToken = default)
        {
            await SendAsync(model, ieFlag, cancellationToken);
        }

        public async Task SendOperateFeedbackAsync(Model_84.OPERATE_FEEDBACK model, string ieFlag, CancellationToken cancellationToken = default)
        {
            await SendAsync(model, ieFlag, cancellationToken);
        }

        public async Task SendManualCheckAsync(Model_85.MANUAL_CHECK model, string ieFlag, CancellationToken cancellationToken = default)
        {
            await SendAsync(model, ieFlag, cancellationToken);
        }

        public async Task SendAsync(TcpCommunicatePacket packet, CancellationToken cancellationToken = default)
        {
            var totalLength = packet.TotalLength;
            using (var tcpClient = new TcpClient())
            {
                //连接
                await tcpClient.ConnectAsync(IPAddress.Parse(options.RemoteHost), options.RemotePort, cancellationToken);
                using (var ns = tcpClient.GetStream())
                {
                    await ns.WriteAsync(sendBuffer, 0, totalLength, cancellationToken);
                    await ns.FlushAsync();
                }
                options.Logger?.Invoke($"已向[{options.RemoteHost}:{options.RemotePort}]发送数据。数据大小：{totalLength}，数据内容：{BitConverter.ToString(sendBuffer, 0, totalLength)}");
                tcpClient.Close();
                tcpClient.Dispose();
            }
            //For self test
            //_ = beginReadFromStream(new MemoryStream(sendBuffer), cancellationToken);
        }

        public async Task SendAsync(string xml, DataType dataType, string areaId, string channelId, string ieFlag, CancellationToken cancellationToken = default)
        {
            var packet = new TcpCommunicatePacket(sendBuffer, false)
            {
                AreaId = areaId,
                ChannelId = channelId,
                DataType = dataType,
                IeFlag = ieFlag
            };
            packet.SetXml(xml);
            await SendAsync(packet);
        }

        public async Task SendAsync<T>(T model, string ieFlag, CancellationToken cancellationToken = default)
            where T : IModel
        {
            var packet = new TcpCommunicatePacket(sendBuffer, false)
            {
                IeFlag = ieFlag
            };
            packet.SetXmlModel(model);
            await SendAsync(packet);
        }
    }
}