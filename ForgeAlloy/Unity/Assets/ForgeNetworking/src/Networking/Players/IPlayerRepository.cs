namespace Forge.Networking.Players
{
	public interface IPlayerRepository
	{
		int Count { get; }
		void AddPlayer(INetPlayer player);
		void RemovePlayer(int playerId);
		void RemovePlayer(INetPlayer player);
		INetPlayer GetPlayer(int id);
	}
}
