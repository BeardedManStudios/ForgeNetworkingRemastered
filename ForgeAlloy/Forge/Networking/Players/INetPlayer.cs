using System;
using System.Net;

namespace Forge.Networking.Players
{
	/// <summary>
	/// Hint:  This interface is designed to work well with the IPlayerRepository interface
	/// as a way to locate and store the player during runtime execution
	/// </summary>
	public interface INetPlayer
	{
		/// <summary>
		/// Is:  A handle to the endpoint that is being used to manage this player
		/// identity. This is what should be used to communicate with this player
		/// </summary>
		EndPoint EndPoint { get; set; }

		/// <summary>
		/// Is:  A unique id for the player that is created and assigned by
		/// the host. This id can be stored for looking up this instance of the player
		/// </summary>
		IPlayerSignature Id { get; set; }

		/// <summary>
		/// Is:  A fairly common thing in most systems for the player to have a non-unique
		/// readable identifyer; sometimes this is a "Gamertag" or "Username" and in
		/// other times this could be a name that is typed in at runtime by the player.
		/// This is just a helper field that is not used within the greater Forge system
		/// for anything specific, assume this field is developer facing only.
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Is:  A boolean that is checked by the engine and the ForgeReadyForEngineInterpreter
		/// to know if the player is already in the engine. This is set by the
		/// ForgeReadyForEngineInterpreter
		/// </summary>
		bool IsInEngine { get; set; }

		/// <summary>
		/// This handles the last time that we communicated with this player, this is useful
		/// for timeouts and checking network health on a given player
		/// </summary>
		DateTime LastCommunication { get; set; }
	}
}
