# Unity Integration Network Start
Since Unity does not work as fast as Forge Networking, there are a few control flow features that we've added into the Unity Integration to allow you to plan your network setup for network objects. When you implement your generated **Behavior** script that was generated from the [NCW](/NetworkContractWizard/network-contract-wizard-ncw.md), you can override a function named **NetworkStart**. When the `NetworkStart` function is called, that means the object has been setup on the network and is ready to do things like send RPCs, update fields, etc. Below is an example of how to override the NetworkStart method in order to disable a camera on a game object that is not owned by the current instance and set our fields update interval.

```csharp
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class NetCam : NetworkCameraBehavior
{
    protected override void NetworkStart()
	{
		base.NetworkStart();

		cameraRef = GetComponent<Camera>();
		networkObject.UpdateInterval = 100;

		// If this is not our camera then we should not render using it
		if (!networkObject.IsOwner)
			cameraRef.enabled = false;
	}
}
```

## networkStarted Event
For your convenience, we've added a networkStarted event to `NetworkBehavior` so that you can do other operations on other objects that have a reference to the object in question. For example; if you instantiate the object (obviously from another script) and you want to do some special action on that object once it has been setup on the network, you can hook into this event.

```csharp
var cameraPlayer = NetworkManager.Instance.InstantiateNetworkCamera();
cameraPlayer.networkStarted += CameraReadyOnNetwork;

// ...

private void CameraReadyOnNetwork(NetworkBehavior playerCamera)
{
    UnityEngine.Debug.Log("The player camera " + playerCamera.name + " has been setup on the network");
}
```
