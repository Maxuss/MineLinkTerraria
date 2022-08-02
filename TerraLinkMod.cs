using System.Threading;
using TerraLink.Network;
using Terraria.ModLoader;

namespace TerraLink
{
	public class TerraLinkMod : Mod
	{
		public static TerraLinkMod Instance { get; private set; }
		public static LinkerClient Client { get; private set; }
		
		public override void Load()
		{
			Instance = this;
			Client = new LinkerClient();
			Logger.Info("Enabling TerraLink...");

			var connectionThread = new Thread(Client.Connect)
			{
				Name = "TerraLink Connector"
			};
			connectionThread.Start();
		}
	}
}