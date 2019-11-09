namespace Forge.Networking.Messaging
{
	public interface IMessageReciever
	{
		void Send(IMessage message);
	}
}
