using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Frame;

namespace BeardedManStudios.Forge.Networking
{
	public interface IServer
	{
		List<string> BannedAddresses { get; set; }
		void Disconnect(NetworkingPlayer player, bool forced);
		void BanPlayer(ulong networkId, int minutes);
		void CommitDisconnects();
		bool AcceptingConnections { get; }
		void StopAcceptingConnections();
		void StartAcceptingConnections();
		void SendReliable(FrameStream frame);
		void SendReliableToPlayer(NetworkingPlayer player, FrameStream frame);
		void SendUnreliable(FrameStream frame);
		void SendUnreliableToPlayer(NetworkingPlayer player, FrameStream frame);
	}
}
