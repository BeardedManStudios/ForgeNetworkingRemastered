using Forge.Engine;
using Forge.Networking.Messaging;

namespace Forge.Networking.Unity
{
	public interface IEngineFacade : IEngineProxy
	{
		string CurrentMap { get; }
		IMessageRepository NewClientMessageBuffer { get; }
		INetworkMediator NetworkMediator { get; set; }
		IPrefabManager PrefabManager { get; }
		int GetNewEntityId();
	}
}
