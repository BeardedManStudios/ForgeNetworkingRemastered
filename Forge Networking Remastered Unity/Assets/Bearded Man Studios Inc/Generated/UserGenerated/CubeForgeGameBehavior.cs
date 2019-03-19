using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"Vector3\", \"Vector3\", \"byte[]\"][\"byte\", \"Vector3\"][\"Vector3\"][\"string\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"minimum\", \"maximum\", \"data\"][\"typebyte\", \"position\"][\"position\"][\"message\"]]")]
	public abstract partial class CubeForgeGameBehavior : NetworkBehavior
	{
		public const byte RPC_INITIALIZE_MAP = 0 + 5;
		public const byte RPC_CREATE_PRIMITIVE = 1 + 5;
		public const byte RPC_DESTROY_PRIMITIVE = 2 + 5;
		public const byte RPC_TEST_ME = 3 + 5;
		
		public CubeForgeGameNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (CubeForgeGameNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("InitializeMap", InitializeMap, typeof(Vector3), typeof(Vector3), typeof(byte[]));
			networkObject.RegisterRpc("CreatePrimitive", CreatePrimitive, typeof(byte), typeof(Vector3));
			networkObject.RegisterRpc("DestroyPrimitive", DestroyPrimitive, typeof(Vector3));
			networkObject.RegisterRpc("TestMe", TestMe, typeof(string));

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId)){
					uint newId = obj.NetworkId + 1;
					ProcessOthers(gameObject.transform, ref newId);
				}
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata != null)
			{
				byte transformFlags = obj.Metadata[0];

				if (transformFlags != 0)
				{
					BMSByte metadataTransform = new BMSByte();
					metadataTransform.Clone(obj.Metadata);
					metadataTransform.MoveStartIndex(1);

					if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() =>
						{
							transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
							transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
						});
					}
					else if ((transformFlags & 0x01) != 0)
					{
						MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
					}
					else if ((transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
					}
				}
			}

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new CubeForgeGameNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new CubeForgeGameNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// Vector3 minimum
		/// Vector3 maximum
		/// byte[] data
		/// </summary>
		public abstract void InitializeMap(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// byte typebyte
		/// Vector3 position
		/// </summary>
		public abstract void CreatePrimitive(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// Vector3 position
		/// </summary>
		public abstract void DestroyPrimitive(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// string message
		/// </summary>
		public abstract void TestMe(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}