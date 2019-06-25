#if !UNITY_WEBGL
#if !NetFX_CORE
#if FACEPUNCH_STEAMWORKS

using Steamworks;
using System;
using System.Collections.Generic;
using System.Net;

namespace BeardedManStudios.Forge.Networking
{
	public class CachedFacepunchP2PClient : IDisposable
	{
		private bool disposed = false;
		private bool active = false;
		protected bool Active
		{
			get { return active; }
			set { active = value; }
		}

		private SteamId steamEndPoint;
		private BMSByte recBuffer = new BMSByte();

		public CachedFacepunchP2PClient()
		{

		}

		public CachedFacepunchP2PClient(SteamId endPoint)
		{
			steamEndPoint = endPoint;
			recBuffer.SetSize(65536);
		}

		/// <summary>
		/// Dispose of this CachedFacepunchP2PClient
		/// </summary>
		public void Close()
		{
			((IDisposable)this).Dispose();
		}

		/// <summary>
		/// Check for and return any packets sent to us
		/// </summary>
		/// <param name="from">SteamId of the player who sent us this packet</param>
		/// <returns>Packet in BMSByte format</returns>
		public BMSByte Receive(out SteamId from)
		{
			CheckDisposed();

			recBuffer.Clear();

			var packet = SteamNetworking.ReadP2PPacket();

			if (packet.HasValue)
			{
				from = packet.Value.SteamId;
				recBuffer.SetSize(packet.Value.Data.Length);
				recBuffer.byteArr = packet.Value.Data;
				return recBuffer;
			}

			from = default(SteamId);
			return null;
		}

		/// <summary>
		/// Actually sends the packet using SteamNetworking
		/// </summary>
		/// <param name="dgram">byte[] to send</param>
		/// <param name="bytes">number of bytes in the packet</param>
		/// <param name="steamId">SteamId of the recipient</param>
		/// <param name="type">Steamworks.P2PSend type</param>
		/// <returns></returns>
		int DoSend(byte[] dgram, int bytes, SteamId steamId, P2PSend type)
		{
			if (SteamNetworking.SendP2PPacket(steamId, dgram, bytes, 0, type) == false)
			{
				Logging.BMSLog.LogWarningFormat("CachedSteamP2PClient:DoSend() WARNING: Unable to send packet to {0}", steamId.Value);
			}

			return 0;
		}

		/// <summary>
		/// Prepare & send packet to SteamId
		/// </summary>
		/// <param name="dgram">byte[] to send</param>
		/// <param name="bytes">number of bytes in the packet</param>
		/// <param name="steamId">SteamId of the recipient</param>
		/// <param name="type">Steamworks.P2PSend type</param>
		/// <returns></returns>
		public int Send(byte[] dgram, int bytes, SteamId steamId, P2PSend type = P2PSend.Unreliable)
		{
			CheckDisposed();
			if (dgram == null)
				throw new ArgumentNullException("dgram is null");

			return (DoSend(dgram, bytes, steamId, type));
		}



		/// <summary>
		/// On disposal of this CachedFacepunchP2PClient IDisposable object
		/// </summary>
		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disconnects from the Steam P2PSession
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return;

			disposed = true;

			if (disposing)
			{
				// Dispose of Steam P2P Socket
				if (!SteamNetworking.CloseP2PSessionWithUser(steamEndPoint))
				{
					if (steamEndPoint != SteamClient.SteamId)
						Logging.BMSLog.LogWarning("Could not close P2P Session with user: " + steamEndPoint.Value);
				}
			}
		}

		~CachedFacepunchP2PClient()
		{
			Dispose(false);
		}

		/// <summary>
		/// Throw an exception if we're trying to use something we've already committed for disposal
		/// </summary>
		private void CheckDisposed()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().FullName);
		}

#if Net_4_5

public Task<UdpReceiveResult> ReceiveAsync ()
{
return Task<UdpReceiveResult>.Factory.FromAsync (BeginReceive, r => {
IPEndPoint remoteEndPoint = null;
return new UdpReceiveResult (EndReceive (r, ref remoteEndPoint), remoteEndPoint);
}, null);
}

public Task<int> SendAsync (byte[] datagram, int bytes)
{
return Task<int>.Factory.FromAsync (BeginSend, EndSend, datagram, bytes, null);
}

public Task<int> SendAsync (byte[] datagram, int bytes, IPEndPoint endPoint)
{
return Task<int>.Factory.FromAsync (BeginSend, EndSend, datagram, bytes, endPoint, null);
}

public Task<int> SendAsync (byte[] datagram, int bytes, string hostname, int port)
{
var t = Tuple.Create (datagram, bytes, hostname, port, this);

return Task<int>.Factory.FromAsync ((callback, state) => {
var d = (Tuple<byte[], int, string, int, UdpClient>) state;
return d.Item5.BeginSend (d.Item1, d.Item2, d.Item3, d.Item4, callback, null);
}, EndSend, t);

}
#endif
	}
}

#endif
#endif
#endif
