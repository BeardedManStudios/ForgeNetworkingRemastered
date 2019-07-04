using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public abstract class NetworkBehavior : MonoBehaviour, INetworkBehavior
	{
		public const byte RPC_SETUP_TRANSFORM = 4;

		public delegate void NetworkBehaviorEvent(NetworkBehavior behavior);

		public static Dictionary<uint, NetworkBehavior> skipAttachIds = new Dictionary<uint, NetworkBehavior>();

		public int TempAttachCode { get; set; }
		public bool Initialized { get; private set; }

		private NetworkObject waitingForNetworkObject;
		private uint waitingForNetworkObjectOffset;

		public event NetworkBehaviorEvent networkStarted;

		protected virtual void NetworkStart()
		{
			CompleteRegistration();

			if (networkStarted != null)
				networkStarted(this);
		}

		protected virtual void CompleteRegistration() { }

		public abstract void Initialize(NetworkObject obj);
		public abstract void Initialize(NetWorker networker, byte[] metadata = null);

		public void AwaitNetworkBind(NetWorker networker, NetworkObject createTarget, uint idOffset)
		{
			waitingForNetworkObject = createTarget;
			waitingForNetworkObjectOffset = idOffset;

			networker.objectCreated += NetworkBind;

			if (createTarget.NetworkId == 0)
				return;

			NetworkObject target;
			if (networker.NetworkObjects.TryGetValue(createTarget.NetworkId + idOffset, out target))
				NetworkBind(target);
		}

		private void NetworkBind(NetworkObject obj)
		{
			// This is always the network manager and should be ignored on late bindings
			if (obj.NetworkId == 0)
				return;

			if (obj.NetworkId != waitingForNetworkObject.NetworkId + waitingForNetworkObjectOffset && obj.CreateCode != TempAttachCode)
				return;

			Initialize(obj);
			skipAttachIds.Remove(obj.NetworkId);

			obj.Networker.objectCreated -= NetworkBind;
		}

		protected void SetupHelperRpcs(NetworkObject networkObject)
		{
			networkObject.RegisterRpc("SetupTransform", SetupTransform, typeof(Vector3), typeof(Quaternion));
			Initialized = true;
		}

		public abstract NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null);

		private void SetupTransform(RpcArgs args)
		{
			Action execute = () =>
			{
				transform.position = args.GetNext<Vector3>();
				transform.rotation = args.GetNext<Quaternion>();
				InitializedTransform();
			};

			if (Rpc.MainThreadRunner != null)
				execute();
			else
				MainThreadManager.Run(execute);
		}

		protected abstract void InitializedTransform();

		[Obsolete("[Forge Networking] Please recompile your scripts in the Network Contract Wizard, your generated code is using a depricated method")]
		protected void ProcessOthers(Transform obj, uint idOffset)
		{
			Debug.LogError("[Forge Networking] Please recompile your scripts in the Network Contract Wizard, your generated code is using a depricated method");
			ProcessOthers(obj, ref idOffset);
		}

		protected void ProcessOthers(Transform obj, ref uint idOffset)
		{
			int i;

			var components = obj.GetComponents<NetworkBehavior>();

			// Create each network object that is available
			for (i = 0; i < components.Length; i++)
			{
				if (components[i] == this)
					continue;

				skipAttachIds.Add(idOffset++, components[i]);
			}

			for (i = 0; i < obj.childCount; i++)
				ProcessOthers(obj.GetChild(i), ref idOffset);
		}
	}
}