using Forge.Serialization;

namespace Forge.Networking.Sockets
{
	public interface ISocket
	{
		void Send(byte[] buffer, int length);
		int Receive(BMSByte buffer);
		void Close();
	}
}
