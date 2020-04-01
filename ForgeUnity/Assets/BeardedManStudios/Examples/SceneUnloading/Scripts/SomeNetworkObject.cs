using System;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class SomeNetworkObject : SomeMoveableBehavior
{
	private byte RPC_SERVER_TEST_RPC;

	private float time;

	protected override void RegisterCustomRPCs(NetworkObject networkObject)
	{
		base.RegisterCustomRPCs(networkObject);

		RPC_SERVER_TEST_RPC = networkObject.RegisterRpc("ServerTestRPC", ServerTestRPC, typeof(int), typeof(string));
	}

	protected override void NetworkStart()
	{
		base.NetworkStart();

		networkObject.SendRpc(RPC_SERVER_TEST_RPC, Receivers.Server, 10, "Some string");
	}

	private void Update()
	{
		if (networkObject == null)
			return;

		if(networkObject.IsOwner)
		{
			time += Time.deltaTime;
			transform.Translate(Mathf.Cos(time) * 0.1f, 0, 0);
			networkObject.position = transform.position;
		}
		else
		{
			transform.position = networkObject.position;
		}
	}

	private void ServerTestRPC(RpcArgs args)
	{
		MainThreadManager.Run(() =>
		{
			int someValue = args.GetNext<int>();
			string someString = args.GetNext<string>();

			Debug.Log("Custom RPC Test: " + someValue + "  " + someString);
		});
	}
}
