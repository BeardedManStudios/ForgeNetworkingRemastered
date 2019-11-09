namespace Forge.Networking.Messaging
{
	public interface IMessageReciever
	{
		void Send(byte[] message);
	}
}
