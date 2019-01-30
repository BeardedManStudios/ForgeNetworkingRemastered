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

		/// <summary>
		/// <para>
		/// Validates the create request from a client. This method will only be called on the server-side.
		/// Override this method with your custom code to validate client object create requests.
		/// </para>
		/// <para>
		/// NOTE: The server is ALWAYS allowed to create objects, and thus this method is never called in this case.
		/// </para>
		/// <para>
		/// If the client should be allowed to create the requested object return <c>true</c>,
		/// otherwise return <c>false</c>. In case of denial of the request, the requesting client can still create
		/// the object locally. However, the server will NOT create a local copy of the object nor propagate that
		/// request to any other connected client.
		/// </para>
		/// </summary>
		/// <param name="networker">The networker that sent the create request</param>
		/// <param name="identity">The ID of the object to be created</param>
		/// <param name="id">The id (if any) given to this object by the server</param>
		/// <param name="frame">The initialization data for this object that is assigned from the server</param>
		/// <returns><c>true</c>, if the client request is deemed valid. Otherwise, <c>false</c>.</returns>
		protected virtual bool ValidateCreateRequest(NetWorker networker, int identity, uint id, FrameStream frame)
		{
			return true;
		}
	}
}
