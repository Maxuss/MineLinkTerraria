using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using MineLink.Network.Packets;

namespace MineLink.Network;

public class LinkerClient
{
    private Channel<IPacket> PacketsTx { get; set; } = Channel.CreateBounded<IPacket>(32);
    private Channel<IPacket> PacketsRx { get; set; } = Channel.CreateBounded<IPacket>(32);
    private BufferedStream Stream { get; set; } = null;
    private TcpClient Client { get; set; } = null;
    private ProtocolState State = ProtocolState.Handshake;
    

    public async Task SendPacket(IPacket packet)
    {
        await PacketsTx.Writer.WriteAsync(packet);
    }

    public async Task<IPacket> ReceivePacket()
    {
        return await PacketsRx.Reader.ReadAsync();
    }

    public async void Connect()
    {
        try
        {
            Client = new TcpClient("127.0.0.1", 25535);
        }
        catch (SocketException err)
        {
            MineLink.Instance.Logger.Error($"Could not connect to bridge: {err.Message}");
            return;
        }

        Stream = new BufferedStream(Client.GetStream(), 8192);
        
        MineLink.Instance.Logger.Info("Initialized TCP Client at 127.0.0.1:25535");
        MineLink.Instance.Logger.Info("Handshaking with server...");

        var writerThread = new Thread(Writer);
        var readerThread = new Thread(Reader);

        writerThread.Name = "MineLink Outgoing";
        readerThread.Name = "MineLink Inbound";
        
        writerThread.Start();
        readerThread.Start();

        await BeginHandshake();
    }

    private async Task BeginHandshake()
    {
        await SendPacket(new ConnectPacket());
        var next = await ReceivePacket();
        switch(next)
        {
            case AdvancePacket advancePacket:
            {
                MineLink.Instance.Logger.Info($"Successfully connected to bridge: {advancePacket.BridgeInfo}!");
                State = ProtocolState.Game;
                break;
            }
            case DisconnectPacket disconnect:
            {
                MineLink.Instance.Logger.Info($"Disconnected from bridge: {disconnect.Message}");
                Client.Dispose();
                break;
            }
            case { } other:
            {
                MineLink.Instance.Logger.Info($"Invalid packet received in handshake: {other}!");
                Client.Dispose();
                break;
            }
        }
    }

    private async void Writer()
    {
        while (Client.Connected)
        {
            var next = await PacketsTx.Reader.ReadAsync();
            next.WritePacket(Stream);
        }
        
        MineLink.Instance.Logger.Warn("Outgoing half disconnected from bridge!");
    }

    private async void Reader()
    {
        while (Client.Connected)
        {
            var packetId = Stream.ReadByte();
            if (packetId == (int)Protocol.PacketId.Disconnect)
                break;
            await PacketsRx.Writer.WriteAsync(Protocol.ReadPacket((byte)packetId, Stream));
        }
        
        MineLink.Instance.Logger.Warn("Inbound half disconnected from bridge!");
    }

    enum ProtocolState
    {
        Handshake,
        Game
    }
}