using System.IO;
using System.Net.Sockets;
using System.Threading.Channels;
using System.Threading.Tasks;
using MineLink.Network.Packets;

namespace MineLink.Network;

public static class Linker
{
    private static Channel<IPacket> PacketsTx { get; set; } = Channel.CreateBounded<IPacket>(32);
    private static Channel<IPacket> PacketsRx { get; set; } = Channel.CreateBounded<IPacket>(32);
    private static BufferedStream Stream { get; set; } = null;
    private static TcpClient Client { get; set; } = null;
    

    public static async void SendPacket(IPacket packet)
    {
        await PacketsTx.Writer.WriteAsync(packet);
    }

    public static async Task<IPacket> ReceivePacket()
    {
        return await PacketsRx.Reader.ReadAsync();
    }

    public static async void Connect()
    {
        Client = new TcpClient("127.0.0.1", 25535);
        Stream = new BufferedStream(Client.GetStream(), 8192);
        
        MineLink.Instance.Logger.Info("Initialized TCP Client at 127.0.0.1:25535");
        MineLink.Instance.Logger.Info("Handshaking with server...");

        while (Client.Connected)
        {
            var packetId = Stream.ReadByte();
        }
    }
}