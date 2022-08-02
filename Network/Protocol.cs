using System.Collections.Generic;
using System.Net.Sockets;
using TerraLink.Network.Packets;

namespace TerraLink.Network;

public static class Protocol
{
    private static Dictionary<byte, IPacketReader<IPacket>> Definition { get; } = new();

    public static void AddPacket(PacketId id, IPacketReader<IPacket> reader)
    {
        Definition[(byte) id] = reader;
    }

    public static IPacket ReadPacket(byte id, NetworkStream stream)
    {
        return Definition[id].ReadPacket(stream);
    }
    
    static Protocol()
    {
        AddPacket(PacketId.Connect, new ConnectPacket.Read());
        AddPacket(PacketId.Disconnect, new DisconnectPacket.Read());
        AddPacket(PacketId.StateAdvance, new AdvancePacket.Read());
    }

    public enum PacketId: byte
    {
        Connect = 0,
        Disconnect = 1,
        StateAdvance = 2,
    }
}