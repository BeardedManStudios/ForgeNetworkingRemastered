using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class SomeNetworkObject : SomeMoveableBehavior
{
	private float time;

	void Update()
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
}
