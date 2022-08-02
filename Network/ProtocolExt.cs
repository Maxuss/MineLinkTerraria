using System;
using System.Buffers.Binary;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace TerraLink.Network;

public static class ProtocolExt
{
    private const int MaxStrLen = short.MaxValue;
    
    public static void WriteStr(this NetworkStream stream, string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        if (bytes.Length >= MaxStrLen)
            TerraLinkMod.Instance.Logger.Warn($"Writing string of size over MaxStrLen ({bytes.Length} > {MaxStrLen}). This might disconnect client from older bridge versions!");
        var len = bytes.Length;
        stream.Write(new[]
        {
            (byte) (len >> 24),
            (byte) (len >> 16),
            (byte) (len >> 8),
            (byte) (len >> 0)
        });
        stream.Write(bytes);
    }

    public static string ReadStr(this NetworkStream stream)
    {
        var lenBuffer = new byte[4];
        _ = stream.Read(lenBuffer);
        var size = BitConverter.ToInt32(lenBuffer);
        var strBuf = new byte[size];
        _ = stream.Read(strBuf);
        return Encoding.UTF8.GetString(strBuf);
    }
}