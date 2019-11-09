namespace Forge.Networking.Messaging
{
	public interface IMessage
	{
		int MessageCode { get; set; }
		IMessageReceipt Receipt { get; set; }
		IMessageInterpreter Interpreter { get; }
		void Interpret(INetworkHost host);
		byte[] Serialize();
	}
}
