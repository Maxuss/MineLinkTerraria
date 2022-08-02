using System.Net.Sockets;

namespace TerraLink.Network.Packets;

public interface IPacket
{
    public IPacketReader<IPacket> Reader { get; }
    public void WritePacket(NetworkStream to);
}

public interface IPacketReader<out TPacket> where TPacket: IPacket
{
    public TPacket ReadPacket(NetworkStream from);
}