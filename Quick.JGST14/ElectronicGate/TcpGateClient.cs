using System.Net;
using System.Net.Sockets;

namespace Quick.JGST14.ElectronicGate;

/// <summary>
/// TCP卡口客户端
/// </summary>
public class TcpGateClient : AbstractGateClient
{
    private byte[] sendBuffer;
    private byte[] recvBuffer;

    private TcpGateClientOptions options;
    private CancellationTokenSource cts;
    private TcpListener tcpListener;

    public TcpGateClient(TcpGateClientOptions options)
    {
        this.options = options;
        sendBuffer = new byte[options.SendBufferSize];
        recvBuffer = new byte[options.RecvBufferSize];
    }

    public override void Start()
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
            var slimHeadMemory = freeRecvMemory.Slice(0, TcpGatePacket.SLIM_HEAD_SIZE);
            await stream.ReadExactlyAsync(slimHeadMemory);
            var totalLength = TcpGatePacket.ParseTotalLength(slimHeadMemory.Span);
            //读取包剩余部分
            await stream.ReadExactlyAsync(recvBuffer, TcpGatePacket.SLIM_HEAD_SIZE, totalLength - TcpGatePacket.SLIM_HEAD_SIZE);
            //解析包
            var packet = new TcpGatePacket(recvBuffer, true);
            switch (packet.DataType)
            {
                case DataType.GatherInfo:
                    RaiseEvent_GatherInfoReceived(packet.GetModelInfo<Model_81.GATHER_INFO>());
                    break;
                case DataType.GatherFeedback:
                    RaiseEvent_GatherFeedbackReceived(packet.GetModelInfo<Model_82.GATHER_FEEDBACK>());
                    break;
                case DataType.OperateInfo:
                    RaiseEvent_OperateInfoReceived(packet.GetModelInfo<Model_83.OPERATE_INFO>());
                    break;
                case DataType.OperateFeedback:
                    RaiseEvent_OperateFeedbackReceived(packet.GetModelInfo<Model_84.OPERATE_FEEDBACK>());
                    break;
                case DataType.ManualCheck:
                    RaiseEvent_ManualCheckReceived(packet.GetModelInfo<Model_85.MANUAL_CHECK>());
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

    public override void Stop()
    {
        cts?.Cancel();
        cts = null;
        tcpListener?.Stop();
        tcpListener = null;
    }

    private async Task SendAsync(TcpGatePacket packet, CancellationToken cancellationToken = default)
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

    public override async Task SendAsync(string xml, CancellationToken cancellationToken = default)
    {
        var model = Deserialize(xml);
        var packet = new TcpGatePacket(sendBuffer, false)
        {
            AreaId = model.AREA_ID,
            ChannelId = model.CHNL_NO,
            DataType = model.GetDataType(),
            IeFlag = model.I_E_FLAG
        };
        packet.SetXml(xml);
        await SendAsync(packet);
    }

    public override async Task SendAsync<T>(T model, CancellationToken cancellationToken = default)
    {
        var packet = new TcpGatePacket(sendBuffer, false);
        packet.SetXmlModel(model);
        await SendAsync(packet);
    }
}