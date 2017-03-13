using UnityEngine;
using System.Collections;
using BeardedManStudios.Forge.Networking.Frame;
using System;

namespace BeardedManStudios.Forge.Networking
{
	public class NetworkObjectFactoryBase : INetworkObjectFactory
	{
		public virtual void NetworkCreateObject(NetWorker networker, int identity, uint id, FrameStream frame, Action<NetworkObject> callback)
		{
			//This is the final creation check before failing completely
			NetworkObject obj = null;

			switch (identity)
			{
				case Lobby.LobbyService.LobbyServiceNetworkObject.IDENTITY:
					obj = new Lobby.LobbyService.LobbyServiceNetworkObject(networker, id, frame);
					break;
			}

			if (callback != null)
				callback(obj);
		}
	}
}
