using BeardedManStudios.Forge.Networking;
using System;
using System.Net;

namespace NatHolePunchServer
{
	public struct Host
	{
		public NetworkingPlayer player;
		public IPEndPoint endpoint;
		public IPAddress address;
		public string host;
		public ushort port;
		
		public Host(NetworkingPlayer player, string host, ushort port)
		{
			this.player = player;
			this.host = host;
			this.port = port;

			if (IPAddress.TryParse(host, out address))
				endpoint = new IPEndPoint(address, port);
			else
				throw new Exception("Invalid host address");
		}
	}
}