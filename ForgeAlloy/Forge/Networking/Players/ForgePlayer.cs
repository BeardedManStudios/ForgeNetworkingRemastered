using System;
using System.Net;

namespace Forge.Networking.Players
{
	public class ForgePlayer : INetPlayer
	{
		public EndPoint EndPoint { get; set; }
		public IPlayerSignature Id { get; set; }
		public string Name { get; set; }
		public bool IsInEngine { get; set; }
		public DateTime LastCommunication { get; set; }
	}
}
