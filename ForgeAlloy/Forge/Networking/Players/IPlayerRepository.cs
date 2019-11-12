using System;
using System.Net;

namespace Forge.Networking.Players
{
	/// <summary>
	/// The event signature for when a player has been added to an implementation
	/// of the IPlayerRepository interface
	/// </summary>
	/// <param name="player">The player that was just added to this repository</param>
	public delegate void PlayerAddedToRepository(INetPlayer player);

	/// <summary>
	/// 
	/// </summary>
	public interface IPlayerRepository
	{
		/// <summary>
		/// Is:  A helper event for communicating when a player has been
		/// added to this repository. This is useful in developer's code for
		/// knowing when to do game-specific implementations such as posting
		/// log messages or instantiating objects to represent the player that
		/// just was added to this repository
		/// </summary>
		event PlayerAddedToRepository onPlayerAdded;

		/// <summary>
		/// Is:  The number of players that are currently held within this
		/// repository. This is useful for showing player counts or knowing
		/// when the game server has reached it's player capacity or possibly
		/// when to shut down based on the number of players. This should be
		/// used for any logic that needs to know about the current network
		/// player instance count
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Should:  Add a player to the repository AND execute the onPlayerAdded
		/// event for listeners to know when a player has been officially added
		/// to this repository
		/// </summary>
		/// <param name="player">
		/// The player that is added ot the repository and should be the player
		/// that is supplied to the onPlayerAdded event
		/// </param>
		void AddPlayer(INetPlayer player);

		/// <summary>
		/// Should:  Remove the player who's ID matches the Guid supplied to this
		/// function. This should also make the Count of the repository reflect this
		/// change as well as have the correct behavior of Exists returning false
		/// on the same ID immediately after this function is invoked
		/// </summary>
		/// <param name="id">
		/// The unique id that is the INetPlayer id of a player in this repository
		/// </param>
		void RemovePlayer(Guid id);

		/// <summary>
		/// Should:  Remove the player who matches the reference supplied to this
		/// function. This should also make the Count of the repository reflect this
		/// change as well as have the correct behavior of Exists returning false
		/// on the same ID immediately after this function is invoked
		/// </summary>
		/// <param name="player">The direct reference to the player that should be removed</param>
		void RemovePlayer(INetPlayer player);


		/// <summary>
		/// Should:  Match a player in the repository with the supplied id and if found
		/// it will return the found player, otherwise it should throw an exception
		/// </summary>
		/// <param name="id">The id which matches the INetPlayer's id within this repository</param>
		/// <returns>The player that was found</returns>
		/// <exception cref="PlayerNotFoundException">
		/// Thrown if the player is not contained within this repository (could not be located)
		/// </exception>
		INetPlayer GetPlayer(Guid id);

		/// <summary>
		/// Should:  Match a player in the repository with the supplied endpoint and if found
		/// it will return the found player, otherwise it should throw an exception
		/// </summary>
		/// <param name="endpoint">
		/// The endpoint that is matched against the player's ISocket::EndPoint
		/// </param>
		/// <returns>The player that was found</returns>
		/// <exception cref="PlayerNotFoundException">
		/// Thrown if the player is not contained within this repository (could not be located)
		/// </exception>
		INetPlayer GetPlayer(EndPoint endpoint);

		/// <summary>
		/// Should:  Look through the repository collection and find a player which has
		/// an ISocket that has a matching ISocket::EndPoint with the supplied endpoint. If
		/// one is located then this method should return true, otherwise return false
		/// </summary>
		/// <param name="endpoint">
		/// The endpoint that is matched against the player's ISocket::EndPoint
		/// </param>
		/// <returns>True if the player matching the endpoint was found, otherwise false</returns>
		bool Exists(EndPoint endpoint);
	}
}
