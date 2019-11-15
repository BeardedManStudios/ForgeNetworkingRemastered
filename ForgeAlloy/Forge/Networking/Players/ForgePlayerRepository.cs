using System.Collections.Generic;
using System.Net;
using Forge.Factory;

namespace Forge.Networking.Players
{
	public class ForgePlayerRepository : IPlayerRepository
	{
		private readonly Dictionary<IPlayerSignature, INetPlayer> _playerLookup = new Dictionary<IPlayerSignature, INetPlayer>();
		private readonly Dictionary<EndPoint, INetPlayer> _playerAddressLookup = new Dictionary<EndPoint, INetPlayer>();

		public event PlayerAddedToRepository onPlayerAddedSubscription;
		public event PlayerAddedToRepository onPlayerRemovedSubscription;

		public int TimeoutMilliseconds { get; set; } = 10000;
		public int Count { get => _playerLookup.Count; }

		public void AddPlayer(INetPlayer player)
		{
			player.Id = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerSignature>();
			_playerLookup.Add(player.Id, player);
			_playerAddressLookup.Add(player.EndPoint, player);
			onPlayerAddedSubscription?.Invoke(player);
		}

		public INetPlayer GetPlayer(IPlayerSignature id)
		{
			if (!_playerLookup.TryGetValue(id, out var player))
				throw new PlayerNotFoundException(id);
			return player;
		}

		public void RemovePlayer(INetPlayer player)
		{
			_playerLookup.Remove(player.Id);
			_playerAddressLookup.Remove(player.EndPoint);
			onPlayerRemovedSubscription?.Invoke(player);
		}

		public void RemovePlayer(IPlayerSignature id)
		{
			var player = _playerLookup[id];
			_playerLookup.Remove(id);
			_playerAddressLookup.Remove(player.EndPoint);
			onPlayerRemovedSubscription?.Invoke(player);
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

		public IEnumerator<INetPlayer> GetEnumerator()
		{
			return _playerLookup.Values.GetEnumerator();
		}
	}
}
