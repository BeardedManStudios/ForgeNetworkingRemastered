using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"string\", \"string\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"username\", \"message\"]]")]
	public abstract partial class ChatManagerBehavior : NetworkBehavior
	{
		public const byte RPC_SEND_MESSAGE = 0 + 5;
		
		public ChatManagerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (ChatManagerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("SendMessage", SendMessage, typeof(string), typeof(string));
			networkObject.RegistrationComplete();

			MainThreadManager.Run(NetworkStart);

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId))
					ProcessOthers(gameObject.transform, obj.NetworkId + 1);
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata == null)
				return;

			byte transformFlags = obj.Metadata[0];

			if (transformFlags == 0)
				return;

			BMSByte metadataTransform = new BMSByte();
			metadataTransform.Clone(obj.Metadata);
			metadataTransform.MoveStartIndex(1);

			if ((transformFlags & 0x01) != 0)
			{
				transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
			}

			if ((transformFlags & 0x02) != 0)
			{
				transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
			}
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new ChatManagerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject()
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new ChatManagerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// string username
		/// string message
		/// </summary>
		public abstract void SendMessage(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}