using System;
using Forge.Networking.Sockets;

namespace Forge.Networking.Players
{
	public interface INetPlayer
	{
		ISocket Socket { get; set; }
		Guid Id { get; set; }
		string Name { get; set; }
	}
}
