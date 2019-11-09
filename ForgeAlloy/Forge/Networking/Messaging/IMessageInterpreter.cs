namespace Forge.Networking.Messaging
{
	public interface IMessageInterpreter
	{
		void Interpret(INetwork netHost, IMessage message);
	}
}
