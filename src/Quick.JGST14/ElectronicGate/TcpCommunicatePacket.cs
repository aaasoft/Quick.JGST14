using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;
using Quick.JGST14.Utils;

namespace Quick.JGST14.ElectronicGate;

public class TcpCommunicatePacket
{
    public const int SLIM_HEAD_SIZE = 8;
    public const int HEAD_SIZE = 38;
    /// <summary>
    /// 包头标记
    /// </summary>
    private static readonly byte[] HEAD_MARK = [0xE2, 0x5C, 0x4B, 0x89];
    /// <summary>
    /// 包体标志
    /// </summary>
    private static byte[] BODY_MARK = [0xFF, 0xFF, 0xFF, 0xFF];
    /// <summary>
    /// 包尾标记
    /// </summary>
    private static byte[] TAIL_MARK = [0xFF, 0xFF];

    private byte[] buffer;

    private Memory<byte> SliceMemory(int length, ref Memory<byte> currentMemory)
    {
        var ret = currentMemory.Slice(0, length);
        currentMemory = currentMemory.Slice(length);
        return ret;
    }

    public static int ParseTotalLength(Span<byte> span)
    {
        var headMark = span.Slice(0, 4);
        if (!IsBufferMatchHeader(headMark, HEAD_MARK))
            throw new IOException($"包头标记[{BitConverter.ToString(headMark.ToArray())}]不正确。");

        var totalLengthMark = span.Slice(4, 4);
        return ToInt32(totalLengthMark);
    }

    public TcpCommunicatePacket(byte[] buffer, bool hasData)
    {
        this.buffer = buffer;

        var currentMemory = new Memory<byte>(buffer);

        HeadMarkMemory = SliceMemory(4, ref currentMemory);
        //检查包头
        if (hasData)
        {
            if (!IsBufferMatchHeader(HeadMarkMemory.Span, HEAD_MARK))
                throw new IOException($"包头标记[{BitConverter.ToString(HeadMarkMemory.Span.ToArray())}]不正确。");
        }
        //设置包头
        else
        {
            HEAD_MARK.CopyTo(HeadMarkMemory);
        }

        TotalLengthMemory = SliceMemory(4, ref currentMemory);
        DataTypeMemory = SliceMemory(1, ref currentMemory);
        AreaIdMemory = SliceMemory(10, ref currentMemory);
        ChannelIdMemory = SliceMemory(10, ref currentMemory);
        IeFlagMemory = SliceMemory(1, ref currentMemory);
        BodyMarkMemory = SliceMemory(4, ref currentMemory);
        //检查标志符
        if (hasData)
        {
            if (!IsBufferMatchHeader(BodyMarkMemory.Span, BODY_MARK))
                throw new IOException($"标志符[{BitConverter.ToString(BodyMarkMemory.Span.ToArray())}]不正确。");
        }
        //设置标志符
        else
        {
            BODY_MARK.CopyTo(BodyMarkMemory);
        }
        XmlStreamLengthMemory = SliceMemory(4, ref currentMemory);
        if (hasData)
        {
            var xmlStreamLength = XmlStreamLength;
            XmlStreamMemory = SliceMemory(xmlStreamLength, ref currentMemory);
            TailMarkMemory = SliceMemory(2, ref currentMemory);
            //检查包尾
            if (!IsBufferMatchHeader(TailMarkMemory.Span, TAIL_MARK))
                throw new IOException($"包尾标记[{BitConverter.ToString(TailMarkMemory.Span.ToArray())}]不正确。");
        }
    }

    private Memory<byte> HeadMarkMemory;

    private Memory<byte> TotalLengthMemory;
    /// <summary>
    /// 总长
    /// </summary>
    public int TotalLength
    {
        get
        {
            return ByteUtils.B2I_BE(TotalLengthMemory);
        }
        set
        {
            var tmpArray = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tmpArray);
            tmpArray.CopyTo(TotalLengthMemory);
        }
    }

    private Memory<byte> DataTypeMemory;
    /// <summary>
    /// 报文代码
    /// </summary>
    public DataType DataType
    {
        get { return (DataType)DataTypeMemory.Span[0]; }
        set { DataTypeMemory.Span[0] = (byte)value; }
    }

    private Memory<byte> AreaIdMemory;
    /// <summary>
    /// 场站号
    /// </summary>
    public string AreaId
    {
        get { return ToString(AreaIdMemory.Span); }
        set { ToSpan(value, AreaIdMemory.Span); }
    }

    private Memory<byte> ChannelIdMemory;
    /// <summary>
    /// 通道号
    /// </summary>
    public string ChannelId
    {
        get { return ToString(ChannelIdMemory.Span); }
        set { ToSpan(value, ChannelIdMemory.Span); }
    }
    private Memory<byte> IeFlagMemory;
    /// <summary>
    /// 进出标志
    /// </summary>
    public string IeFlag
    {
        get { return char.ConvertFromUtf32(IeFlagMemory.Span[0]); }
        set { IeFlagMemory.Span[0] = Encoding.Default.GetBytes(value)[0]; }
    }

    private Memory<byte> BodyMarkMemory;
    private Memory<byte> XmlStreamLengthMemory;
    /// <summary>
    /// XML流长度
    /// </summary>
    public int XmlStreamLength
    {
        get => ToInt32(XmlStreamLengthMemory.Span);
        set => ToSpan(value, XmlStreamLengthMemory.Span);
    }

    private Memory<byte> XmlStreamMemory;
    private Memory<byte> TailMarkMemory;



    private static bool IsBufferMatchHeader(Span<byte> buffer, Span<byte> header)
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

    private string ToString(Span<byte> span)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < span.Length; i++)
            sb.Append(span[i].ToString());
        return sb.ToString();
    }

    private void ToSpan(string value, Span<byte> span)
    {
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            span[i] = byte.Parse(c.ToString());
        }
    }

    private static int ToInt32(Span<byte> span)
    {
        var ret = BitConverter.ToInt32(span);
        //如果是小端字节序，则交换
        if (BitConverter.IsLittleEndian)
        {
            var rawSpan = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref ret, 1));
            rawSpan.Reverse();
        }
        return ret;
    }

    private void ToSpan(int value, Span<byte> span)
    {
        var rawSpan = MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1));
        rawSpan.CopyTo(span);
        if (BitConverter.IsLittleEndian)
            span.Reverse();
    }

    public void SetXmlModel<T>(T t)
        where T : IModel
    {
        //设置报文代码
        DataType = t.GetDataType();
        //设置场站号
        AreaId = t.AREA_ID;
        //设置通道号
        ChannelId = t.CHNL_NO;

        var xmlStreamLength = 0;
        using (var ms = new MemoryStream(buffer, HEAD_SIZE, buffer.Length - HEAD_SIZE))
        {
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(ms, t);
            xmlStreamLength = (int)ms.Position;
        }
        var totalLength = HEAD_SIZE + xmlStreamLength + TAIL_MARK.Length;
        //设置总长度
        TotalLength = totalLength;
        //设置XML流长度
        XmlStreamLength = xmlStreamLength;
        //设置包尾
        TAIL_MARK.CopyTo(TailMarkMemory);
    }

    public T GetXmlModel<T>()
        where T : IModel
    {
        using (var ms = new MemoryStream(buffer, HEAD_SIZE, XmlStreamLength))
        {
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(ms);
        }
    }

    public ModelInfo<T> GetModelInfo<T>() where T : IModel
    {
        return new ModelInfo<T>()
        {
            DataType = DataType,
            AreaId = AreaId,
            ChannelId = ChannelId,
            IeFlag = IeFlag,
            Data = GetXmlModel<T>()
        };
    }
}
