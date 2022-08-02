using System.Threading;
using MineLink.Network;
using Terraria.ModLoader;

namespace MineLink
{
	public class MineLink : Mod
	{
		public static MineLink Instance { get; private set; }
		public static LinkerClient Client { get; private set; }
		
		public override void Load()
		{
			Instance = this;
			Client = new LinkerClient();
			Logger.Info("Enabling MineLink...");

			var connectionThread = new Thread(Client.Connect)
			{
				Name = "MineLink Connector"
			};
			connectionThread.Start();
		}
	}
}