using Forge.Engine;
using Forge.Networking.Messaging;

namespace Forge.Networking.Unity
{
	public delegate void EntitiesLoadedEvent();

	public interface IEngineFacade : IEngineProxy
	{
		bool IsServer { get; }
		string CurrentMap { get; }
		IMessageRepository NewClientMessageBuffer { get; }
		INetworkMediator NetworkMediator { get; set; }
		IPrefabManager PrefabManager { get; }
		IEntityRepository EntityRepository { get; }
		int GetNewEntityId();
	}
}
