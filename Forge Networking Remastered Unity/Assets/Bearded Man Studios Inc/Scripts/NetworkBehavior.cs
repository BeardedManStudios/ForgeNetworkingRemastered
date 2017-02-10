using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public abstract class NetworkBehavior : MonoBehaviour, INetworkBehavior
	{
		public static List<uint> skipAttachIds = new List<uint>();

		public int TempAttachCode { get; set; }
		public bool Initialized { get; private set; }

		private uint waitingForNetworkObjectId;

		protected virtual void NetworkStart() { }
		public abstract void Initialize(NetworkObject obj);
		public abstract void Initialize(NetWorker networker);

		public void AwaitNetworkBind(NetWorker networker, uint id)
		{
			// NetworkManager always has the id of 0 reserved, so 0 is an invalid binding
			if (id == 0)
				throw new Exception("An identifyer is required to be greater than 0");

			waitingForNetworkObjectId = id;
			NetworkObject.objectCreated += NetworkBind;

			NetworkObject target;
			if (networker.NetworkObjects.TryGetValue(id, out target))
				NetworkBind(target);
		}

		private void NetworkBind(NetworkObject obj)
		{
			// This is always the network manager and should be ignored on late bindings
			if (obj.NetworkId == 0)
				return;

			if (obj.NetworkId != waitingForNetworkObjectId && obj.CreateCode != TempAttachCode)
				return;

			Initialize(obj);
			skipAttachIds.Remove(obj.NetworkId);

			NetworkObject.objectCreated -= NetworkBind;
		}

		protected void SetupHelperRpcs(NetworkObject networkObject)
		{
			networkObject.RegisterRpc("SetupPosition", SetupPosition, typeof(Vector3));
			networkObject.RegisterRpc("SetupTransform", SetupTransform, typeof(Vector3), typeof(Quaternion));
			Initialized = true;
		}

		public abstract NetworkObject CreateNetworkObject(NetWorker networker, int createCode);

		private void SetupPosition(RpcArgs args)
		{
			MainThreadManager.Run(() =>
			{
				transform.position = args.GetNext<Vector3>();
				InitializedTransform();
			});
		}

		private void SetupTransform(RpcArgs args)
		{
			MainThreadManager.Run(() =>
			{
				transform.position = args.GetNext<Vector3>();
				transform.rotation = args.GetNext<Quaternion>();
				InitializedTransform();
			});
		}

		protected virtual void InitializedTransform() { }
	}
}