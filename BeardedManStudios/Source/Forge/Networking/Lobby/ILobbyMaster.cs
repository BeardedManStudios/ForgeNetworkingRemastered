using System;
using System.Collections.Generic;

namespace BeardedManStudios.Forge.Networking.Lobby
{
    /// <summary>
    /// The purpose of the ILobbyMaster is to be able to hold
    /// the information of all players and update them accordingly
    /// </summary>
    public interface ILobbyMaster
    {
        /// <summary>
        /// This is the list of all the lobby players
        /// </summary>
        List<IClientMockPlayer> LobbyPlayers { get; }
        /// <summary>
        /// This is the lobby player map of each user based on id
        /// </summary>
        Dictionary<uint, IClientMockPlayer> LobbyPlayersMap { get; }
        /// <summary>
        /// This is the list of lobby players associated by their team
        /// </summary>
        Dictionary<int, List<IClientMockPlayer>> LobbyTeams { get; }
        /// <summary>
        /// Called when a player has connected
        /// </summary>
        /// <param name="player">The connecting player</param>
        void OnFNPlayerConnected(IClientMockPlayer player);
        /// <summary>
        /// Called when a player has disconnected
        /// </summary>
        /// <param name="player">The disconnecting player</param>
        void OnFNPlayerDisconnected(IClientMockPlayer player);
        /// <summary>
        /// Called when a player has changed their name
        /// </summary>
        /// <param name="player">The player who's name has been updated</param>
        void OnFNPlayerNameChanged(IClientMockPlayer player);
        /// <summary>
        /// Called when a player has changed their team
        /// </summary>
        /// <param name="player">The player who changed their team</param>
        void OnFNTeamChanged(IClientMockPlayer player);
        /// <summary>
        /// Called when a player has changed their avatar id
        /// </summary>
        /// <param name="player">The player who changed his avatar id</param>
        void OnFNAvatarIDChanged(IClientMockPlayer player);
        /// <summary>
        /// Called when a player has synced with the server
        /// </summary>
        /// <param name="player">The player to update</param>
        void OnFNPlayerSync(IClientMockPlayer player);
        /// <summary>
        /// This is only called when you change the lobby service
        /// </summary>
        /// <param name="previousLobbyMaster">The previous lobby that we should pull values from</param>
        void OnFNLobbyMasterKnowledgeTransfer(ILobbyMaster previousLobbyMaster);
        /// <summary>
        /// This is only called when there has been a message received from a player
        /// </summary>
        /// <param name="player">Player who sent the message</param>
        /// <param name="message">The message the player sent</param>
        void OnFNLobbyPlayerMessageReceived(IClientMockPlayer player, string message);
    }
}
