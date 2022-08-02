using System.IO;

namespace MineLink.Network.Packets;

public interface IPacket
{
    public IPacketReader<IPacket> Reader { get; }
    public void WritePacket(BufferedStream to);
}

public interface IPacketReader<out TPacket> where TPacket: IPacket
{
    public TPacket ReadPacket(BufferedStream from);
}