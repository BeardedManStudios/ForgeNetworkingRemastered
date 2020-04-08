namespace Forge.Networking.Players
{
	public interface IPlayerTimeoutBridge
	{
		int TimeoutMilliseconds { get; set; }
		void StartWatching(INetworkMediator mediator);
	}
}
