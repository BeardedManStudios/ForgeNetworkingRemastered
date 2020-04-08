using System;

namespace Forge.Networking.Messaging
{
	[Serializable]
	public class MessageBussNetworkMediatorAlreadyAssignedException : Exception
	{
		public MessageBussNetworkMediatorAlreadyAssignedException()
			: base($"The specified forge message bus already had been assigned a network mediator which can't be re-assigned")
		{

		}
	}
}