using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace MineLink.Network;

public static class ProtocolExt
{
    private const int MaxStrLen = short.MaxValue;
    
    public static void WriteStr(this BufferedStream stream, string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        if (bytes.Length >= MaxStrLen)
            MineLink.Instance.Logger.Warn($"Writing string of size over MaxStrLen ({bytes.Length} > {MaxStrLen}). This might disconnect client from older bridge versions!");
        stream.Write(BitConverter.GetBytes(bytes.Length));
        stream.Write(bytes);
    }

    public static string ReadStr(this BufferedStream stream)
    {
        var lenBuffer = new byte[4];
        _ = stream.Read(lenBuffer);
        var size = BitConverter.ToInt32(lenBuffer);
        var strBuf = new byte[size];
        _ = stream.Read(strBuf);
        return Encoding.UTF8.GetString(strBuf);
    }
}