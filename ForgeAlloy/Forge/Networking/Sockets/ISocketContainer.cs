using System.Threading;

namespace Forge.Networking.Sockets
{
	public interface ISocketFacade
	{
		/// <summary>
		/// The token for this socket's thread to know when it has been canceld. This
		/// can be used on any other threads that are spawned in relation to this socket
		/// </summary>
		CancellationTokenSource CancellationSource { get; }

		ISocket ManagedSocket { get; }
		void ShutDown();
		void Established(INetworkMediator netMediator);
	}
}
