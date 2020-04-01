using Forge.Engine;

namespace ForgeServerRegistryService.Engine
{
	public class ServerRegistryEngine : IEngineProxy
	{
		public void NetworkingEstablished()
		{
			// Not expecting to have any entities on the server registry engine
			return;
		}
	}
}
