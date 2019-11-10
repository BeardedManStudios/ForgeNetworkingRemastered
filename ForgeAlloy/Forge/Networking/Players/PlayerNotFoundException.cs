using System;
using System.Net;

namespace Forge.Networking.Players
{
	public class PlayerNotFoundException : Exception
	{
		public PlayerNotFoundException(Guid id)
			: base($"A player with the id {id} could not be found in the repository")
		{

		}
		public PlayerNotFoundException(EndPoint ep)
			: base($"A player with the endpoint {ep} could not be found in the repository")
		{

		}
	}
}
