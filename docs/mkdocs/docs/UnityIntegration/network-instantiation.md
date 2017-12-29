# Network Instantiation
Network instantiation is how you are able to create objects on the network so that all other clients are able to see that object. Then you can update this object or send [Remote Procedure Calls](basic-rpc-example) to the object to perform behaviors across the network or manage local behaviors.

When you use the [Network Contract Wizard](network-contract-wizard-ncw) you are able to generate what is known as a "Network Object". These objects can then be extended to have custom behavior and to implement network behaviors defined in the Network Contract Wizard (NCW). When you click the "Save & Compile" option in the NCW an instantiate method will be created for the object you just defined in the NCW. So if you created a Network Object in the NCW named **SuperBall** then you could call the following code in Unity to create an instance of that **SuperBall**:

```csharp
NetworkManager.Instance.InstantiateSuperBall();
```

If you look at the input parameters of this method you will find that there are a few optional parameters:

```
1. int index = 0
2. Vector3? position = null
3. Quaternion? rotation = null
4. bool sendTransform = true
```

1. The index tells the network which prefab to instantiate (from the array set in the Network Manager prefab).
2. This is the position you wish to spawn the prefab at on the network. If left null then the object will spawn at the coordinates set in the prefab.
3. This is the rotation you wish to spawn the prefab at on the network. If left null then the object will spawn at the rotation set in the prefab.
4. This is used to know if the transform that is being assigned should be sent across the network. So if you filled out #2 and #3 then those values will be sent across the network and not just used locally. If you pass `false` then the prefab will spawn on the caller at the specified location, but it will be spawned at the prefab location on the other clients.

You may recall from the above #1 that you can set various prefabs to spawn for a given network object. That is to say that my **SuperBall** could have more than one prefab it spawns on, even a cube (I know crazy right?). To work with this you will need to locate the **Network Manager** prefab in the `Bearded Man Studios Inc/Prefabs` folder. Once you select this prefab, you will notice that there is a `NetworkPrefab` component attached to it with a **Super Ball Network Object** array that is empty. This is where you will drag and drop the various prefabs you wish to use as this type of network object. So we can drag a sphere and even a cube into this array.

Let's say that we first put a sphere into the array and then we put a cube into the array. You will notice that the sphere has an index of **0** and the cube will have an index of **1**. This is the value that you will supply to the instantiate method when you call it. If you were to pass **0**, you would get a sphere and if you were to pass a **1** you would get a cube.

## Getting the GameObject From Instantiate
What you may notice is that when you instantiate the **SuperBall** it will return a **NetworkBehavior** which will be your **SuperBallBehavior** that was generated. This object actually derives from the `MonoBehaviour` so you can directly access the gameObject, transform, and any standard `MonoBehaviour` field, property, and method as you would do normally.

```csharp
var ball = NetworkManager.Instance.InstantiateSuperBall();

// Access any standard MonoBehaviour elements as seen below from the ball reference
Debug.Log(ball.gameObject.name);
Debug.Log(ball.transform.position);
```

----------

## Notes
So you may be thinking, well gee, a numbered index might be a little bit hard to keep track of, and if you do, then you are probably correct. Like all things software there are many ways that you can setup your project, but here are just 2 ideas that you can use to have a little better time managing the various prefabs you may use.

### Method 1
You can create a class that is dedicated to mapping objects to strings or integers. Then you can access the `NetworkManager.Instance` and assign the array directly with `NetworkManager.Instance.SuperBallNetworkObject = myArray;`. By doing this you can create your own indexing and lookup logic for the prefabs that you can use to easily find the index for the prefab that should spawn on the network. After creating the mapping you just need to call your lookup method and have it do the rest of the work.

### Method 2
You can create your own instantiate methods. You can wrap code around our code to spawn your objects. So imagine that you want to call the generated **NetworkManager.Instance.InstantiateSuperBall** method but you have some other stuff you always do before hand or that dynamically selects the proper index. In this case you can create a class that does all the helper logic before you call the instantiate method, then you would call the instantiate through this helper class.

### Manual Attachment Method
You can create a NetworkObject without having to call this insantiate method which is directly linked to a prefab. This obviously isn't a topic for this section but you can find out more about this in the **Getting Started** examples about network insantiation.

## Warnings
If you are dynamically setting the array on the **NetworkManager.Instance** then you **MUST** make sure that the order being assigned is predictable on all clients. You can't exactly have the order of the array different on each client and expect it to still work properly can you?
