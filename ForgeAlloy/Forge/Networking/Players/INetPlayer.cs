using Forge.Networking.Sockets;

namespace Forge.Networking.Players
{
	public interface INetPlayer
	{
		ISocket Socket { get; }
		int Id { get; }
		string Ip { get; }
		ushort Port { get; }
		string Name { get; }
	}
}
