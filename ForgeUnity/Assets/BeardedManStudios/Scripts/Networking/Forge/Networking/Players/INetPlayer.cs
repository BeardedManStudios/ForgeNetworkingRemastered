namespace BeardedManStudios.Forge.Networking.Players
{
	public interface INetPlayer
	{
		int Id { get; }
		string Ip { get; }
		ushort Port { get; }
		string Name { get; }
		PlayerConnectState ConnectState { get; }
		ulong LastPing { get; }
		bool IsHost { get; }
	}
}
