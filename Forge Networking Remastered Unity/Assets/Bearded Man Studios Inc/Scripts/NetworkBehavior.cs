using System.Collections.Generic;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public abstract class NetworkBehavior : MonoBehaviour, INetworkBehavior
	{
		public static List<uint> skipAttachIds = new List<uint>();

		public int TempAttachCode { get; set; }
		public bool Initialized { get; private set; }

		protected virtual void NetworkStart() { }
		public abstract void Initialize(NetworkObject obj);
		public abstract void Initialize(NetWorker networker);

		private uint waitingForNetworkObjectId;
		public void WaitingForNetworkObject(NetWorker networker, uint id)
		{
			waitingForNetworkObjectId = id;
			NetworkObject.objectCreated += NetworkObjectReceived;

			NetworkObject target;
			if (networker.NetworkObjects.TryGetValue(id, out target))
				NetworkObjectReceived(target);
		}

		private void NetworkObjectReceived(NetworkObject obj)
		{
			if (obj.NetworkId != waitingForNetworkObjectId)
				return;

			Initialize(obj);

			NetworkObject.objectCreated -= NetworkObjectReceived;
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