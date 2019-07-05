using System;

namespace BeardedManStudios.Forge.Networking.SQP
{
	[Flags]
	public enum ChunkType
	{
		ServerInfo = 1,
		ServerRules = 2,
		PlayerInfo = 4,
		TeamInfo = 8
	}
}
