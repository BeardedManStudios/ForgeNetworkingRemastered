using BeardedManStudios.Forge.Networking.Generated;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Unity
{
	public partial class NetworkManager : MonoBehaviour
	{
		public delegate void InstantiateEvent(INetworkBehavior unityGameObject, NetworkObject obj);
		public event InstantiateEvent objectInitialized;

		public GameObject[] ChatManagerNetworkObject = null;
		public GameObject[] CubeForgeGameNetworkObject = null;
		public GameObject[] ExampleProximityPlayerNetworkObject = null;
		public GameObject[] NetworkCameraNetworkObject = null;

		private void Start()
		{
			NetworkObject.objectCreated += (obj) =>
			{
				if (obj.CreateCode < 0)
					return;
				
				if (obj is ChatManagerNetworkObject)
				{
					MainThreadManager.Run(() =>
					{
						NetworkBehavior newObj = null;
						if (!NetworkBehavior.skipAttachIds.TryGetValue(obj.NetworkId, out newObj))
						{
							if (ChatManagerNetworkObject.Length > 0 && ChatManagerNetworkObject[obj.CreateCode] != null)
							{
								var go = Instantiate(ChatManagerNetworkObject[obj.CreateCode]);
								newObj = go.GetComponent<NetworkBehavior>();
							}
						}

						if (newObj == null)
							return;
						
						newObj.Initialize(obj);

						if (objectInitialized != null)
							objectInitialized(newObj, obj);
					});
				}
				else if (obj is CubeForgeGameNetworkObject)
				{
					MainThreadManager.Run(() =>
					{
						NetworkBehavior newObj = null;
						if (!NetworkBehavior.skipAttachIds.TryGetValue(obj.NetworkId, out newObj))
						{
							if (CubeForgeGameNetworkObject.Length > 0 && CubeForgeGameNetworkObject[obj.CreateCode] != null)
							{
								var go = Instantiate(CubeForgeGameNetworkObject[obj.CreateCode]);
								newObj = go.GetComponent<NetworkBehavior>();
							}
						}

						if (newObj == null)
							return;
						
						newObj.Initialize(obj);

						if (objectInitialized != null)
							objectInitialized(newObj, obj);
					});
				}
				else if (obj is ExampleProximityPlayerNetworkObject)
				{
					MainThreadManager.Run(() =>
					{
						NetworkBehavior newObj = null;
						if (!NetworkBehavior.skipAttachIds.TryGetValue(obj.NetworkId, out newObj))
						{
							if (ExampleProximityPlayerNetworkObject.Length > 0 && ExampleProximityPlayerNetworkObject[obj.CreateCode] != null)
							{
								var go = Instantiate(ExampleProximityPlayerNetworkObject[obj.CreateCode]);
								newObj = go.GetComponent<NetworkBehavior>();
							}
						}

						if (newObj == null)
							return;
						
						newObj.Initialize(obj);

						if (objectInitialized != null)
							objectInitialized(newObj, obj);
					});
				}
				else if (obj is NetworkCameraNetworkObject)
				{
					MainThreadManager.Run(() =>
					{
						NetworkBehavior newObj = null;
						if (!NetworkBehavior.skipAttachIds.TryGetValue(obj.NetworkId, out newObj))
						{
							if (NetworkCameraNetworkObject.Length > 0 && NetworkCameraNetworkObject[obj.CreateCode] != null)
							{
								var go = Instantiate(NetworkCameraNetworkObject[obj.CreateCode]);
								newObj = go.GetComponent<NetworkBehavior>();
							}
						}

						if (newObj == null)
							return;
						
						newObj.Initialize(obj);

						if (objectInitialized != null)
							objectInitialized(newObj, obj);
					});
				}
			};
		}

		private void InitializedObject(INetworkBehavior behavior, NetworkObject obj)
		{
			if (objectInitialized != null)
				objectInitialized(behavior, obj);

			obj.pendingInitialized -= InitializedObject;
		}

		public ChatManagerBehavior InstantiateChatManagerNetworkObject(int index = 0, Vector3? position = null, Quaternion? rotation = null, bool sendTransform = true)
		{
			var go = Instantiate(ChatManagerNetworkObject[index]);
			var netBehavior = go.GetComponent<NetworkBehavior>() as ChatManagerBehavior;
			var obj = netBehavior.CreateNetworkObject(Networker, index);
			go.GetComponent<ChatManagerBehavior>().networkObject = (ChatManagerNetworkObject)obj;

			FinializeInitialization(go, netBehavior, obj, position, rotation, sendTransform);
			
			return netBehavior;
		}

		public CubeForgeGameBehavior InstantiateCubeForgeGameNetworkObject(int index = 0, Vector3? position = null, Quaternion? rotation = null, bool sendTransform = true)
		{
			var go = Instantiate(CubeForgeGameNetworkObject[index]);
			var netBehavior = go.GetComponent<NetworkBehavior>() as CubeForgeGameBehavior;
			var obj = netBehavior.CreateNetworkObject(Networker, index);
			go.GetComponent<CubeForgeGameBehavior>().networkObject = (CubeForgeGameNetworkObject)obj;

			FinializeInitialization(go, netBehavior, obj, position, rotation, sendTransform);
			
			return netBehavior;
		}

		public ExampleProximityPlayerBehavior InstantiateExampleProximityPlayerNetworkObject(int index = 0, Vector3? position = null, Quaternion? rotation = null, bool sendTransform = true)
		{
			var go = Instantiate(ExampleProximityPlayerNetworkObject[index]);
			var netBehavior = go.GetComponent<NetworkBehavior>() as ExampleProximityPlayerBehavior;
			var obj = netBehavior.CreateNetworkObject(Networker, index);
			go.GetComponent<ExampleProximityPlayerBehavior>().networkObject = (ExampleProximityPlayerNetworkObject)obj;

			FinializeInitialization(go, netBehavior, obj, position, rotation, sendTransform);
			
			return netBehavior;
		}

		public NetworkCameraBehavior InstantiateNetworkCameraNetworkObject(int index = 0, Vector3? position = null, Quaternion? rotation = null, bool sendTransform = true)
		{
			var go = Instantiate(NetworkCameraNetworkObject[index]);
			var netBehavior = go.GetComponent<NetworkBehavior>() as NetworkCameraBehavior;
			var obj = netBehavior.CreateNetworkObject(Networker, index);
			go.GetComponent<NetworkCameraBehavior>().networkObject = (NetworkCameraNetworkObject)obj;

			FinializeInitialization(go, netBehavior, obj, position, rotation, sendTransform);
			
			return netBehavior;
		}

	}
}