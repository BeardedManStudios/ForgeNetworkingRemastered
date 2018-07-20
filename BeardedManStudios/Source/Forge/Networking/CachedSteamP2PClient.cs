//
// System.Net.Sockets.UdpClient.cs
//
// Author:
//	Gonzalo Paniagua Javier <gonzalo@ximian.com>
//	Sridhar Kulkarni (sridharkulkarni@gmail.com)
//	Marek Safar (marek.safar@gmail.com)
//
// Modified by:
//	Brent Farris (brent@beardedmangames.com)
//
// Copyright (C) Ximian, Inc. http://www.ximian.com
// Copyright 2011 Xamarin Inc.
//

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if !UNITY_WEBGL
#if !NetFX_CORE
#if STEAMWORKS

using Steamworks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace BeardedManStudios.Forge.Networking
{
    public class CachedSteamP2PClient : IDisposable
	{
		public const char HOST_PORT_CHARACTER_SEPARATOR = '+';
		private bool disposed = false;
		private bool active = false;
        private CSteamID steamEndPoint;

#region Instantiation
        public CachedSteamP2PClient()
		{
            //steamendpoint = self
		}

        public CachedSteamP2PClient(CSteamID endPoint)
        {
            steamEndPoint = endPoint;
            recBuffer.SetSize(65536);
        }

#endregion
#region Close
        public void Close()
		{
			((IDisposable)this).Dispose();
		}
#endregion
#region Data I/O
		private BMSByte recBuffer = new BMSByte();
		private Dictionary<EndPoint, string> connections = new Dictionary<EndPoint, string>();
		public BMSByte Receive(uint msgSize, out CSteamID from)
		{
            uint dataRead = 0;
			CheckDisposed();

			recBuffer.Clear();
            recBuffer.SetSize((int)msgSize);


            if(SteamNetworking.ReadP2PPacket(recBuffer.byteArr, msgSize, out dataRead, out from))
            {
                return recBuffer;
            }

            return null;
		}

        int DoSend(byte[] dgram, int bytes, CSteamID steamId, EP2PSend type)
        {
            /* Catch EACCES and turn on SO_BROADCAST then,
			 * as UDP Sockets don't have it set by default
			 */
            //if(steamId.IsValid())
            if(SteamNetworking.SendP2PPacket(steamId, dgram, (uint)bytes, type) == false)
            {
                Logging.BMSLog.LogWarningFormat("CachedSteamP2PClient:DoSend() WARNING: Unable to send packet to {0}", steamId.m_SteamID);
            }

            return 0;
        }

        /*public int Send(byte[] dgram, int bytes)
		{
			CheckDisposed();
			if (dgram == null)
				throw new ArgumentNullException("dgram");

			if (!active)
				throw new InvalidOperationException("Operation not allowed on " +
					"non-connected Sockets.");

			return (DoSend(dgram, bytes, null));
		}*/

        public int Send(byte[] dgram, int bytes, CSteamID steamId, EP2PSend type = EP2PSend.k_EP2PSendUnreliable)
        {
            CheckDisposed();
            if (dgram == null)
                throw new ArgumentNullException("dgram is null");

            return (DoSend(dgram, bytes, steamId, type));
        }

		private byte[] CutArray(byte[] orig, int length)
		{
			byte[] newArray = new byte[length];
			Buffer.BlockCopy(orig, 0, newArray, 0, length);

			return newArray;
		}
#endregion

#region Properties
		protected bool Active
		{
			get { return active; }
			set { active = value; }
		}

#endregion
#region Disposing
		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed)
				return;
			disposed = true;

			if (disposing)
			{
                //DISPOSE OF STEAM P2P SOCKET
                if (steamEndPoint.IsValid())
                {
                    SteamNetworking.CloseP2PSessionWithUser(steamEndPoint);
                }
			}
		}

		~CachedSteamP2PClient()
		{
			Dispose(false);
		}

		private void CheckDisposed()
		{
			if (disposed)
				throw new ObjectDisposedException(GetType().FullName);
		}
#endregion

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