using System;

namespace Forge.Networking.Players
{
	public class PlayerNotFoundException : Exception
	{
		public PlayerNotFoundException(int id) : base($"A player with the id {id} could not be found in the repository")
		{

		}
	}
}
