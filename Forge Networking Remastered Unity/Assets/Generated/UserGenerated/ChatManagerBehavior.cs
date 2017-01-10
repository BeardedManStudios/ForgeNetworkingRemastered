using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"string\", \"string\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"username\", \"message\"]]")]
	public abstract partial class ChatManagerBehavior : NetworkBehavior
	{
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
		}

		public override void Initialize(NetWorker networker)
		{
			Initialize(new ChatManagerNetworkObject(networker, createCode: TempAttachCode));
		}

		private void DestroyGameObject()
		{
			MainThreadManager.Run(() => { Destroy(gameObject); });
			networkObject.onDestroy -= DestroyGameObject;
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