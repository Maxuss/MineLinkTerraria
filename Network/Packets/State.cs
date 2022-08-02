using System;
using System.IO;
using System.Text;

namespace MineLink.Network.Packets;

public class DisconnectPacket : IPacket
{
    public string Message { get; init; }
    
    public IPacketReader<IPacket> Reader { get; } = new Read();
    public void WritePacket(BufferedStream to)
    {
        to.WriteByte((byte) Protocol.PacketId.Disconnect);
        to.WriteStr(Message);
    }

    public struct Read: IPacketReader<DisconnectPacket>
    {
        public DisconnectPacket ReadPacket(BufferedStream from)
        {
            // 0x00
            var reason = from.ReadStr();
            return new DisconnectPacket
            {
                Message = reason
            };
        }
    }
}

public class ConnectPacket : IPacket
{
    public IPacketReader<IPacket> Reader { get; } = new Read();
    public void WritePacket(BufferedStream to)
    {
        to.WriteByte((byte) Protocol.PacketId.Connect);
        to.WriteStr($"TModLoader/{MineLink.Instance.TModLoaderVersion}");
    }

    public struct Read : IPacketReader<ConnectPacket>
    {
        public ConnectPacket ReadPacket(BufferedStream from)
        {
            throw new Exception("Can not read ServerConnectPacket!");
        }
    }
}

public class AdvancePacket : IPacket
{
    public string BridgeInfo { get; init; }
    
    public IPacketReader<IPacket> Reader { get; }
    public void WritePacket(BufferedStream to)
    {
        throw new Exception("Can not send StateAdvancePacket!");
    }

    public struct Read : IPacketReader<AdvancePacket>
    {
        public AdvancePacket ReadPacket(BufferedStream from)
        {
            var version = from.ReadStr();
            return new AdvancePacket
            {
                BridgeInfo = version
            };
        }
    }
}