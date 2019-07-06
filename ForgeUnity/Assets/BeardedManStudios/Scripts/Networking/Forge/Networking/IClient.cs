using BeardedManStudios.Forge.Networking.Frame;

namespace BeardedManStudios.Forge.Networking
{
	public interface IClient
	{
		NetworkingPlayer ServerPlayer { get; }
		void SendReliable(FrameStream frame);
		void SendUnreliable(FrameStream frame);
	}
}
