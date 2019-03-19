/*-----------------------------+-------------------------------\
|                                                              |
|                         !!!NOTICE!!!                         |
|                                                              |
|  These libraries are under heavy development so they are     |
|  subject to make many changes as development continues.      |
|  For this reason, the libraries may not be well commented.   |
|  THANK YOU for supporting forge with all your feedback       |
|  suggestions, bug reports and comments!                      |
|                                                              |
|                              - The Forge Team                |
|                                Bearded Man Studios, Inc.     |
|                                                              |
|  This source code, project files, and associated files are   |
|  copyrighted by Bearded Man Studios, Inc. (2012-2017) and    |
|  may not be redistributed without written permission.        |
|                                                              |
\------------------------------+------------------------------*/

using BeardedManStudios.Forge.Networking.Frame;
using System;
using System.Runtime.InteropServices;

namespace BeardedManStudios.Forge.Networking
{
	public class TCPClientWebsockets : TCPClientBase
	{
        //TODO: Brent, figure out how to processor define this in a dll for unity
        // or we need to make 2 dlls, one with and one without this pre-processored defined out
        // This breaks iOS support because it cannot do [DllImport..]
#if !UNITY_IOS
		[DllImport("__Internal")]
		private static extern void ForgeConnect(string host, ushort port);

		[DllImport("__Internal")]
		private static extern void ForgeWrite(byte[] data, int length);

		[DllImport("__Internal")]
		private static extern IntPtr ForgeShiftDataRead();

		[DllImport("__Internal")]
		private static extern int ForgeContainsData();

		[DllImport("__Internal")]
		private static extern void ForgeClose();

		[DllImport("__Internal")]
		private static extern void ForgeLog(string data);

		[DllImport("__Internal")]
		private static extern bool CheckSocketConnection();

		public override void Connect(string host, ushort port = DEFAULT_PORT)
		{
			//Set the port
			SetPort(port);

			ForgeConnect(host, port);
		}

		public void CheckConnection()
		{
			if (!CheckSocketConnection())
				return;

			// This was a successful bind so fire the events for it
			OnBindSuccessful();

			// Setup the identity of the server as a player
			server = new NetworkingPlayer(0, null, true, null, this);
		}

		/// <summary>
		/// The direct byte send method to the specified client
		/// </summary>
		/// <param name="client">The target client that will receive the frame</param>
		/// <param name="frame">The frame that is to be sent to the specified client</param>
		public override void Send(FrameStream frame)
		{
			// Get the raw bytes from the frame and send them
			byte[] data = frame.GetData();

			ForgeWrite(data, data.Length);
		}

		public override void Disconnect(bool forced)
		{
			ForgeClose();

			// Send signals to the methods registered to the disconnec events
			if (!forced)
				OnDisconnected();
			else
				OnForcedDisconnect();

			for (int i = 0; i < Players.Count; ++i)
				OnPlayerDisconnected(Players[i]);
		}

		public void UpdateStep()
		{
			if (!IsBound)
			{
				CheckConnection();
				return;
			}

			int length = ForgeContainsData();
			if (length == 0)
				return;

			IntPtr ptr = ForgeShiftDataRead();
			byte[] bytes = new byte[length];
			Marshal.Copy(ptr, bytes, 0, bytes.Length);

			if (bytes.Length > 0)
			{
				byte messageType = 130;

				// Get the frame that was sent by the server, the server
				// does not send masked data, only the client so send false for mask
				FrameStream frame = Factory.ReadFrameStream(messageType, bytes, 0, MessageGroupIds.TCP_FIND_GROUP_ID, Server);

				// A message has been successfully read from the network so relay that
				// to all methods registered to the event
				OnMessageReceived(Server, frame);
			}
		}
#endif
	}
}