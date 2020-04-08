using System.Net;
using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	/// <summary>
	/// Hint:  It is important to note that ISocket is NOT a player identity, instead
	/// INetPlayer is the player identity and should be used as such
	/// </summary>
	public interface ISocket
	{
		/// <summary>
		/// Is:  The endpoint for the socket that implements this interface
		/// as it will be required to send/recieve data using this handle
		/// </summary>
		EndPoint EndPoint { get; }

		/// <summary>
		/// Should:  Send an array of bytes (as a packet) to the supplied ISocket.
		/// Because ISocket has an "EndPoint", we are able to use that to know where
		/// the byte array should be sent
		/// </summary>
		/// <param name="target">This is the socket that will recieve the byte data</param>
		/// <param name="buffer">The array of bytes that are to be sent to the target</param>
		void Send(EndPoint endpoint, BMSByte buffer);

		/// <summary>
		/// Should:  Use the supplied buffer to store the bytes supplied from the remote
		/// location. The supplied "endpoint" is used to know where the bytes came from. This
		/// is helpful in looking up which ISocket sent the data as it can be matched to
		/// an ISocket::EndPoint member.
		/// </summary>
		/// <param name="buffer">
		/// Where the bytes that are expected to be read will be put into. So you are expected
		/// to read this object after this call in order to get the bytes that were read
		/// </param>
		/// <param name="endpoint">
		/// The endpoint that the bytes were read from (sender). This interface is not responsible
		/// for finding the ISocket instance that relates to this endpoint and should be done
		/// somewhere else
		/// </param>
		/// <returns>The number of bytes recieved through this socket handle</returns>
		int Receive(BMSByte buffer, ref EndPoint endpoint);

		/// <summary>
		/// Should:  Do any cleanup on the ISocket implementation required, this is useful
		/// for closing or disposing any managed members of the implementation
		/// </summary>
		void Close();
	}
}
