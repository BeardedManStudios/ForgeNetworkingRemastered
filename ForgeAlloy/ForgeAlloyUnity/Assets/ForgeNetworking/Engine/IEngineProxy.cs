namespace Forge.Engine
{
	public interface IEngineProxy
	{
		IForgeLogger Logger { get; }
		void NetworkingEstablished();
	}
}
