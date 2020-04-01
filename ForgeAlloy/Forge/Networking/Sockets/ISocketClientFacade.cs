namespace Forge.Networking.Sockets
{
	public interface ISocketClientFacade : ISocketFacade
	{
		void StartClient(string address, ushort port, INetworkMediator netContainer);
		void Established(INetworkMediator netMediator);
	}
}
