using System;

namespace Quick.JGST14.Utils
{
    public class ByteUtils
    {        
        /// <summary>
        /// 字节数组 -> 整型数字(4字节)(大端字节序)
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="startIndex">起始字节序号(可选)</param>
        /// <returns></returns>
        public static int B2I_BE(Memory<byte> buffer)
        {
            var span = buffer.Span;
            
            //如果是小端字节序，则交换
            if (BitConverter.IsLittleEndian)
                span.Reverse();

            var ret = BitConverter.ToInt32(span);

            //如果是小端字节序，则交换
            if (BitConverter.IsLittleEndian)
                span.Reverse();

            return ret;
        }
    }
}
