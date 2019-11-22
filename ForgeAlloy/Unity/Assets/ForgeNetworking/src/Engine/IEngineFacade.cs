using Forge.Engine;

namespace Forge.Networking.Unity
{
	public interface IEngineFacade : IEngineProxy
	{
		INetworkMediator NetworkMediator { get; set; }
	}
}
