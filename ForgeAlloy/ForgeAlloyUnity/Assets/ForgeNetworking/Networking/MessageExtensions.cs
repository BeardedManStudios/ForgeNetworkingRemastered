using System.Net;
using Forge.Networking.Messaging;

namespace Forge.Networking
{
	public static class MessageExtensions
	{
		public static void ForwardToOtherClients(this INetworkMediator netMediator,
			EndPoint sender, IMessage message)
		{
			if (!netMediator.IsServer)
			{
				throw new SocketNotServerException("Tried to forward a message" +
					"to connected clients, but the current socket is not a server");
			}
			var itr = netMediator.PlayerRepository.GetEnumerator();
			while (itr.MoveNext())
			{
				if (itr.Current != null && itr.Current.EndPoint != sender)
					netMediator.SendReliableMessage(message, itr.Current.EndPoint);
			}
		}

		public static void RunMessageLocally(
			this INetworkMediator netMediator, IMessage message)
		{
			var interpreter = message.Interpreter;
			if (ShouldInterpret(netMediator, interpreter))
			{
				interpreter.Interpret(netMediator,
					netMediator.SocketFacade.ManagedSocket.EndPoint, message);
			}
		}

		private static bool ShouldInterpret(
			INetworkMediator mediator, IMessageInterpreter interpreter)
		{
			return (mediator.IsClient && interpreter.ValidOnClient)
				|| (mediator.IsServer && interpreter.ValidOnServer);
		}
	}
}
