namespace Forge.Networking.Messaging
{
	public interface IMessageInterpreter
	{
		void Interpret(INetworkHost netHost, IMessage message);
	}
}
