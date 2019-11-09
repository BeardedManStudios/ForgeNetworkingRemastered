namespace Forge.Networking.Messaging
{
	public interface IMessageClient
	{
		void Send(byte[] message);
	}
}
