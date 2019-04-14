using System.Collections.Generic;

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
    }
}
