namespace Forge.Networking.Sockets
{
	public interface IClientSocket
	{
		void Connect(string address, ushort port);
	}
}
