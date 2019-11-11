using System;
using System.Collections.Generic;
using System.Net;

namespace Forge.Networking.Players
{
	public class ForgePlayerRepository : IPlayerRepository
	{
		private readonly Dictionary<Guid, INetPlayer> _playerLookup = new Dictionary<Guid, INetPlayer>();
		private readonly Dictionary<EndPoint, INetPlayer> _playerAddressLookup = new Dictionary<EndPoint, INetPlayer>();

		public event PlayerAddedToRepository onPlayerAdded;

		public int Count { get => _playerLookup.Count; }

		public void AddPlayer(INetPlayer player)
		{
			player.Id = Guid.NewGuid();
			_playerLookup.Add(player.Id, player);
			_playerAddressLookup.Add(player.Socket.EndPoint, player);
			onPlayerAdded?.Invoke(player);
		}

		public INetPlayer GetPlayer(Guid id)
		{
			if (!_playerLookup.TryGetValue(id, out var player))
				throw new PlayerNotFoundException(id);
			return player;
		}

		public void RemovePlayer(INetPlayer player)
		{
			_playerLookup.Remove(player.Id);
			_playerAddressLookup.Remove(player.Socket.EndPoint);
		}

		public void RemovePlayer(Guid id)
		{
			var player = _playerLookup[id];
			_playerLookup.Remove(id);
			_playerAddressLookup.Remove(player.Socket.EndPoint);
		}

		public INetPlayer GetPlayer(EndPoint endpoint)
		{
			if (!_playerAddressLookup.TryGetValue(endpoint, out var player))
				throw new PlayerNotFoundException(endpoint);
			return player;
		}

		public bool Exists(EndPoint endpoint)
		{
			return _playerAddressLookup.TryGetValue(endpoint, out _);
		}
	}
}
