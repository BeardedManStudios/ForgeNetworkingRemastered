using System.Net;
using Forge.Engine;
using Forge.Factory;
using Forge.Networking.Messaging.Messages;

namespace ForgeServerRegistryService.Engine
{
	public class ServerRegistryEngine : IEngineProxy
	{
		public IEntityRepository EntityRepository { get; set; }

		public ServerRegistryEngine()
		{
			EntityRepository = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IEntityRepository>();
		}

		public void ProcessUnavailableEntityMessage(IEntityMessage message, EndPoint sender)
		{
			// Not expecting to have any entities on the server registry engine
			return;
		}

		public void NetworkingEstablished()
		{
			// Not expecting to have any entities on the server registry engine
			return;
		}
	}
}
