using System;
using System.Net;

namespace Forge.Networking.Players
{
	public delegate void PlayerAddedToRepository(INetPlayer player);

	public interface IPlayerRepository
	{
		event PlayerAddedToRepository onPlayerAdded;
		int Count { get; }
		void AddPlayer(INetPlayer player);
		void RemovePlayer(Guid id);
		void RemovePlayer(INetPlayer player);
		INetPlayer GetPlayer(Guid id);
		INetPlayer GetPlayer(EndPoint endpoint);
		bool Exists(EndPoint endpoint);
	}
}
