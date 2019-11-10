using System;
using System.Net;

namespace Forge.Networking.Players
{
	public interface IPlayerRepository
	{
		int Count { get; }
		void AddPlayer(INetPlayer player);
		void RemovePlayer(Guid id);
		void RemovePlayer(INetPlayer player);
		INetPlayer GetPlayer(Guid id);
		INetPlayer GetPlayer(EndPoint endpoint);
	}
}
