using Forge.Engine;

namespace Forge.Networking.Unity
{
	public interface IUnityEntity : IEntity
	{
		int CreationId { get; set; }
	}
}
