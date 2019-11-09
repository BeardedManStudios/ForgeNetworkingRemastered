namespace Forge.Networking.Messaging
{
	public interface IMessageInterpreter
	{
		void Interpret(INetworkContainer netHost, IMessage message);
	}
}
