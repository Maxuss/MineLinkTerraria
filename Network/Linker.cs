using System.Net.Sockets;
using System.Threading.Tasks;
using TerraLink.Network.Packets;

namespace TerraLink.Network;

public class LinkerClient
{
    private NetworkStream Stream { get; set; } = null;
    private TcpClient Client { get; set; } = null;
    private ProtocolState State = ProtocolState.Handshake;
    

    public void SendPacket(IPacket packet)
    {
        packet.WritePacket(Stream);
    }

    public async Task<IPacket> ReceivePacket()
    {
        var packetId = Stream.ReadByte();
        return Protocol.ReadPacket((byte)packetId, Stream);
    }

    public async void Connect()
    {
        try
        {
            Client = new TcpClient("127.0.0.1", 25535);
        }
        catch (SocketException err)
        {
            TerraLinkMod.Instance.Logger.Error($"Could not connect to bridge: {err.Message}");
            return;
        }

        Stream = Client.GetStream();

        TerraLinkMod.Instance.Logger.Info("Initialized TCP Client at 127.0.0.1:25535");
        TerraLinkMod.Instance.Logger.Info("Handshaking with server...");
        
        await BeginHandshake();
    }

    private async Task BeginHandshake()
    {
        SendPacket(new ConnectPacket());
        var next = await ReceivePacket();
        switch(next)
        {
            case AdvancePacket advancePacket:
            {
                TerraLinkMod.Instance.Logger.Info($"Successfully connected to bridge: {advancePacket.BridgeInfo}!");
                State = ProtocolState.Game;
                break;
            }
            case DisconnectPacket disconnect:
            {
                TerraLinkMod.Instance.Logger.Info($"Disconnected from bridge: {disconnect.Message}");
                Client.Dispose();
                break;
            }
            case { } other:
            {
                TerraLinkMod.Instance.Logger.Info($"Invalid packet received in handshake: {other}!");
                Client.Dispose();
                break;
            }
        }
    }
    
    enum ProtocolState
    {
        Handshake,
        Game
    }
}