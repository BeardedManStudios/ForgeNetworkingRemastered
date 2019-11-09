using System;

namespace Forge.Networking.Messaging
{
	public class InvalidMessageRepositoryTTLProvided : Exception
	{
		public InvalidMessageRepositoryTTLProvided(int ttlMilliseconds)
			: base($"A TTL is required to be a positive number greater than 0 but {ttlMilliseconds} was provided")
		{

		}
	}
}
