namespace BeardedManStudios.Forge.Networking
{
	public struct RPCInfo
	{
		public NetworkingPlayer SendingPlayer { get; set; }
		public ulong TimeStep { get; set; }
	}
}
