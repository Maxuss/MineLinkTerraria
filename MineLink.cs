using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace MineLink
{
	public class MineLink : Mod
	{
		public static MineLink Instance { get; private set; }
		
		public override void Load()
		{
			Instance = this;
			Logger.Info("Enabling MineLink...");

			var connectionThread = new Thread(BridgeConnector);
			connectionThread.Name = "ML-Bridge Connection Thread";
			connectionThread.Start();
		}

		private static async void BridgeConnector()
		{
			var client = new TcpClient("127.0.0.1", 25535);
			var stream = new BufferedStream(client.GetStream(), 8192);
		}
	}
}