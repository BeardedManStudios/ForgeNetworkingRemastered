using System;
using System.Threading;

namespace BeardedManStudios.Forge.Networking
{
	/// <summary>
	/// The purpose of this master client is for the master server to register
	/// </summary>
	public class TCPMasterClient : TCPClient
	{
		protected override void Initialize(string host, ushort port, bool pendCreates = false)
		{
            base.Initialize(host, port, pendCreates);
            base.InitializeTCPClient(host, port);
		}
	}
}
