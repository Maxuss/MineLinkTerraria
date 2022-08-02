using System.IO;

namespace MineLink.Network.Packets;

public struct DisconnectPacket : IPacket
{
    public IPacketReader<IPacket> Reader { get; } = 
    public void WritePacket(BufferedStream to)
    {
        throw new System.NotImplementedException();
    }

    private struct PacketReader: IPacketReader<DisconnectPacket>
    {
        public DisconnectPacket ReadPacket(BufferedStream from)
        {
            
        }
    }
}