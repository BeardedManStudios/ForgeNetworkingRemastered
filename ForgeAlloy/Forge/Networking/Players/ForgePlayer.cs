using System;
using Forge.Networking.Sockets;

namespace Forge.Networking.Players
{
	public class ForgePlayer : INetPlayer
	{
		public ISocket Socket { get; set; }
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}
