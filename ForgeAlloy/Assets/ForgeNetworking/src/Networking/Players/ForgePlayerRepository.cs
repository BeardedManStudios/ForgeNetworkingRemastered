using System.Collections.Generic;

namespace Forge.Networking.Players
{
	public class ForgePlayerRepository : IPlayerRepository
	{
		private readonly Dictionary<int, INetPlayer> _playerLookup = new Dictionary<int, INetPlayer>();

		public int Count { get => _playerLookup.Count; }

		public ForgePlayerRepository()
		{

		}

		public void AddPlayer(INetPlayer player)
		{
			_playerLookup.Add(player.Id, player);
		}

		public INetPlayer GetPlayer(int id)
		{
			if (!_playerLookup.TryGetValue(id, out var player))
			{
				throw new PlayerNotFoundException(id);
			}
			return player;
		}

		public void RemovePlayer(INetPlayer player)
		{
			_playerLookup.Remove(player.Id);
		}

		public void RemovePlayer(int playerId)
		{
			_playerLookup.Remove(playerId);
		}
	}
}
