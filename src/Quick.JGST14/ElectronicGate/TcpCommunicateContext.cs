using System;
using System.Buffers;
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

        private bool isBufferMatchHeader(ReadOnlySequence<byte> sequence, Span<byte> buffer, byte[] header)
        {
            if (buffer == null || header == null)
                return false;
            sequence.CopyTo(buffer);
            if (buffer.Length != header.Length)
                return false;
            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != header[i])
                    return false;
            }
            return true;
        }

        //解析包总长度
        private int parsePackageTotalLength(ReadOnlySequence<byte> sequence, byte[] buffer)
        {
            sequence.CopyTo(buffer);
            var packageTotalLength = ByteUtils.B2I_BE(buffer, 0);
            return packageTotalLength;
        }

        private string parseDecimalString(Span<byte> span)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < span.Length; i++)
                sb.Append(span[i].ToString());
            return sb.ToString();
        }

        private T Deserialize<T>(byte[] buffer, int start, int length)
        {
            using (var ms = new MemoryStream(buffer, start, length))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(ms);
            }
        }

        private async Task beginReadClient(TcpClient tcpClient, CancellationToken cancellationToken)
        {
            options.Logger?.Invoke($"[{tcpClient.Client.RemoteEndPoint}]已经连接，开始从接收数据。。。");
            var pipe = new System.IO.Pipelines.Pipe();
            var reader = pipe.Reader;
            var writer = pipe.Writer;
            //从网络流中读取，写入到管道中
            _ = Task.Run(async () =>
            {
                try
                {
                    var buffer = new byte[1024];
                    using (var ns = tcpClient.GetStream())
                    {
                        var ret = await ns.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        if (ret > 0)
                        {
                            await writer.WriteAsync(new System.ReadOnlyMemory<byte>(buffer, 0, ret), cancellationToken);
                            await writer.FlushAsync(cancellationToken);
                        }
                    }
                }
                catch(Exception ex)
                {
                    options.Logger?.Invoke($"从[{tcpClient.Client.RemoteEndPoint}]接收数据时出错，原因：{ex}");
                    tcpClient.Dispose();
                }
                finally
                {
                    await writer.CompleteAsync();
                }
            });
            //从管道中读取数据
            _ = Task.Run(async () =>
            {
                var buffer = new byte[1024];
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        //读取包头
                        var ret = await reader.ReadAtLeastAsync(packetStartHead.Length, cancellationToken);
                        //如果包头不匹配
                        if (!isBufferMatchHeader(ret.Buffer, buffer, packetStartHead))
                            break;
                        reader.AdvanceTo(ret.Buffer.End);

                        //读取包长度
                        ret = await reader.ReadAtLeastAsync(4, cancellationToken);
                        var packetTotalLength = parsePackageTotalLength(ret.Buffer, buffer);
                        reader.AdvanceTo(ret.Buffer.End);

                        //读取报文代码
                        ret = await reader.ReadAtLeastAsync(1, cancellationToken);
                        ret.Buffer.CopyTo(buffer);
                        reader.AdvanceTo(ret.Buffer.End);
                        var modelType = (ModelType)buffer[0];

                        //读取场站号
                        ret = await reader.ReadAtLeastAsync(10, cancellationToken);
                        ret.Buffer.CopyTo(buffer);
                        reader.AdvanceTo(ret.Buffer.End);
                        var areaId = parseDecimalString(new Span<byte>(buffer, 0, 10));

                        //读取通道号
                        ret = await reader.ReadAtLeastAsync(10, cancellationToken);
                        ret.Buffer.CopyTo(buffer);
                        reader.AdvanceTo(ret.Buffer.End);
                        var channelId = parseDecimalString(new Span<byte>(buffer, 0, 10));

                        //读取进出标志
                        ret = await reader.ReadAtLeastAsync(1, cancellationToken);
                        ret.Buffer.CopyTo(buffer);
                        reader.AdvanceTo(ret.Buffer.End);
                        var ieFlag = Encoding.Default.GetString(buffer, 0, 1);

                        //读取标识符
                        ret = await reader.ReadAtLeastAsync(packetTagHead.Length, cancellationToken);
                        //如果标识符不匹配
                        if (!isBufferMatchHeader(ret.Buffer, buffer, packetTagHead))
                            break;
                        reader.AdvanceTo(ret.Buffer.End);

                        //读取XML流长度
                        ret = await reader.ReadAtLeastAsync(4, cancellationToken);
                        var xmlTotalLength = parsePackageTotalLength(ret.Buffer, buffer);
                        reader.AdvanceTo(ret.Buffer.End);

                        //读取XML格式数据
                        ret = await reader.ReadAtLeastAsync(xmlTotalLength, cancellationToken);
                        ret.Buffer.CopyTo(buffer);
                        reader.AdvanceTo(ret.Buffer.End);

                        switch (modelType)
                        {
                            case ModelType.GatherInfo:
                                GatherInfoReceived?.Invoke(this, Deserialize<Model_81.GATHER_INFO>(buffer, 0, xmlTotalLength));
                                break;
                            case ModelType.GatherFeedback:
                                GatherFeedbackReceived?.Invoke(this, Deserialize<Model_82.GATHER_FEEDBACK>(buffer, 0, xmlTotalLength));
                                break;
                            case ModelType.OperateInfo:
                                OperateInfoReceived?.Invoke(this, Deserialize<Model_83.OPERATE_INFO>(buffer, 0, xmlTotalLength));
                                break;
                            case ModelType.OperateFeedback:
                                OperateFeedbackReceived?.Invoke(this, Deserialize<Model_84.OPERATE_FEEDBACK>(buffer, 0, xmlTotalLength));
                                break;
                            case ModelType.ManualCheck:
                                ManualCheckReceived?.Invoke(this, Deserialize<Model_85.MANUAL_CHECK>(buffer, 0, xmlTotalLength));
                                break;
                        }

                        //读取包尾
                        ret = await reader.ReadAtLeastAsync(packetEndTail.Length, cancellationToken);
                        //如果包尾不匹配
                        if (!isBufferMatchHeader(ret.Buffer, buffer, packetEndTail))
                            break;
                        reader.AdvanceTo(ret.Buffer.End);
                    }
                }
                catch (Exception ex)
                {
                    options.Logger?.Invoke($"从[{tcpClient.Client.RemoteEndPoint}]解析数据时出错，原因：{ex}");
                    tcpClient.Dispose();
                }
                finally
                {
                    await reader.CompleteAsync();
                }
            });
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
            using (var tcpClient = new TcpClient())
            {
                //连接
                await tcpClient.ConnectAsync(IPAddress.Parse(options.RemoteHost), options.RemotePort, cancellationToken);
                using(var ns = tcpClient.GetStream())
                {
                    //写入包头
                    await ns.WriteAsync(packetStartHead,cancellationToken);
                    //写入
                    
                }
            }
        }
    }
}