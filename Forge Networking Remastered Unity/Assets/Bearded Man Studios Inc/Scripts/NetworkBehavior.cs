using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public abstract class NetworkBehavior : MonoBehaviour, INetworkBehavior
	{
		public int TempAttachCode { get; set; }
		public bool Initialized { get; private set; }

		protected virtual void NetworkStart() { }
		public abstract void Initialize(NetworkObject obj);
		public abstract void Initialize(NetWorker networker);

		protected void SetupHelperRpcs(NetworkObject networkObject)
		{
			networkObject.RegisterRpc("SetupPosition", SetupPosition, typeof(Vector3));
			networkObject.RegisterRpc("SetupTransform", SetupTransform, typeof(Vector3), typeof(Quaternion));
			Initialized = true;
		}

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