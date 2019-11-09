using Forge.Networking.Sockets;

namespace Forge.Networking.Players
{
	public interface INetPlayer
	{
		ISocket Socket { get; }
		int Id { get; }
		string Name { get; }
	}
}
