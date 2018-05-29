# Basic RPC Example

**This example is not a continuation of other examples and should be treated as if it were done in a new project.**

In this example we are going to go over how to use the built in RPC methods inside of Forge Networking Remastered. In this example we are going to make a scene that already has a cube in it, then if anyone presses the up arrow key it will move the cube up, if anyone presses the down arrow key, it will move the cube down.

## Planning Network Code

So, one of the first things we want to think about is our **Network Contract**. This is basically a fancy word for how we design/setup network communication. It is helpful to imagine our final result to our application so that we can properly design for it. In this application when a client connects it will see a cube, if that client or any other connection (server or client) presses the up or down arrow key the cube will move in the appropriate direction for everyone connected:

1. Have a cube already in the scene
2. Press up arrow key to move the cube up via RPC method
3. Press the down arrow key to move the cube down via RPC method

## Network Contract Wizard

Now that we know that we need to sync the **position** and **rotation** of a cube, we can design our network contract for that object. We will first open the **Network Contract Wizard** which is a UI provided by the Bearded Man Studios team to make it easy to design your **network contracts** in a easy way. To open this menu, go into Unity and select "Window->Forge Networking->Network Contract Wizard".

![opening-the-ncw](https://raw.githubusercontent.com/BeardedManStudios/ForgeNetworkingRemastered/develop/docs/mkdocs/docs/images/opening-ncw.jpg "How to open the NCW")

Once you have opened this editor you will be presented with a list of all the Network Objects currently available, to learn more about this please see the document on the Network Contract Wizard (NCW) as we will just be going over how to create a network object through the contract wizard. To start, click on the "Create" button near the top and you will be presented with the create UI. In here, we have 3 major fields of interest, the **Name** fields, the **Fields** field, and the **Remote Procedure Calls** field.

1. The **Name** field is where we create the name for our Network Object and behavior, this is a friendly name that should be written in "Pascal case" to follow the C# convention since it is going to be a part of the class names that are generated.
2. The **Fields** section shows all of the various fields that our network should be aware of.
3. The **Remote Procedure Calls** field is where you will design any Remote Procedure Call (RPC) function signatures. This is going to be our main area of focus for this example.

### Setting up the contract option 1

In this option, we will create 2 RPC methods with no arguments. One RPC is to move the cube up and the other is to move the cube down. **NOTE: Only pick option 1 or 2 to follow**

Let's begin by naming our Network Object:

1. Let's set the name for our Network object to **MoveCube**
2. Click the **Add RPC** button
3. Name the new RPC **MoveUp**
4. Click the **Add RPC** button
5. Name the new RPC **MoveDown**
6. Click the **Save &amp; Compile** button

![move-cube-network-contract-option-1](https://raw.githubusercontent.com/BeardedManStudios/ForgeNetworkingRemastered/develop/docs/mkdocs/docs/images/move-cube-1.jpg "Move Cube Network Contract Option 1")

### Setting up the contract option 2

In this option, we will create 1 RPC with 1 argument that denotes the direction to move the cube. **NOTE: Only pick option 1 or 2 to follow**

Let's begin by naming our Network Object:

1. Let's set the name for our Network object to **MoveCube**
2. Click the **Add RPC** button
3. Name the new RPC **Move**
4. Click the arrow to open **Arguments** next to the RPC name input box
5. Click the green button that appears below the RPC name input box
6. Select **Vector3** from the dropdown option list
7. Click the **Save &amp; Compile** button

![move-cube-network-contract-option-2](https://raw.githubusercontent.com/BeardedManStudios/ForgeNetworkingRemastered/develop/docs/mkdocs/docs/images/move-cube-2.jpg "Move Cube Network Contract Option 2")

In this example, you may be curious what the **Vector3** selection was for. This particular selection was to set the data types that will be sent as arguments to this method. Since we will be sending a direction to move in, this is a **Vector3** , that means that we would need to pick it. The order of these type selections (if we had more than one) are explicit. That is to say that the order you select them in, is the order that you would pass argument types in, just as if you were writing a method in C#.

## Extending Generated Classes

When we use the **Network Contract Wizard (NCW)** we are actually generating a lot of network code based on what has been input into the editor fields, this actually cuts out a lot of work that you would have to do by hand. There is one class that we want to extend from, this class name will be " **MoveCubeBehavior**". The naming convention for this generated class is \_\_\_\_\_Behavior where "\_\_\_\_\_" is the name we typed into the NCW. Let's now create a C# file in Unity and write our basic game logic, we will name this file " **MoveCube**".

1. Open the newly created C# file
2. Add **using BeardedManStudios.Forge.Networking.Generated;** to the using statements
3. Derive the class from **BasicCubeBehavior**
4. Write the rest of the logic for the cube as seen below

### Code if option 1 was selected
####MoveCube
```csharp
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
public class MoveCube : MoveCubeBehavior
{
	private void Update()
	{
		// Move the cube up in world space if the up arrow was pressed
		if (Input.GetKeyDown(KeyCode.UpArrow))
			networkObject.SendRpc(RPC_MOVE_UP, Receivers.All);
		
		// Move the cube down in world space if the down arrow was pressed
		else if (Input.GetKeyDown(KeyCode.DownArrow))
			networkObject.SendRpc(RPC_MOVE_DOWN, Receivers.All);
	}
    
	/// <summary>
	/// Used to move the cube that this script is attached to up
	/// </summary>
	/// <param name="args">null</param>
	public override void MoveUp(RpcArgs args)     
	{
		// RPC calls are not made from the main thread for performance, since we
		// are interacting with Unity enginge objects, we will need to make sure
		// to run the logic on the main thread
		MainThreadManager.Run(() =>
		{
			transform.position += Vector3.up;
		});
    }
	
    /// <summary>
	/// Used to move the cube that this script is attached to down
	/// </summary>
	/// <param name="args">null</param>
	public override void MoveDown(RpcArgs args)     
	{
		// RPC calls are not made from the main thread for performance, since we
		// are interacting with Unity engine objects, we will need to make sure
		// to run the logic on the main thread
		MainThreadManager.Run(() =>
		{
			transform.position += Vector3.down;
		});
	}
}
```

As you can see from the code snippet above an RPC is called using the **networkObject.SendRPC** method. The first argument is the name of the method and the second argument is the receivers of the object which could be set to things like AllBuffered, Others, etc. The moment the RPC method is called it is sent on the network to be replicated to the other clients (including server if called from a client). **Note**: _In this example, it doesn't use a buffered call and it does not actually synchronize the position, so the client should be connected before the cube is moved. **Note 2**: _Notice that we use **MainThreadManager** to run the move logic in this example, if you have not used the **MainThreadManager** before or would like more information about threading in Unity, please view [this page](threading-in-unity)._

### Code if option 2 was selected
#### MoveCube
```csharp
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
public class MoveCube : MoveCubeBehavior {
	private void Update()
	{
		// Move the cube up in world space if the up arrow was pressed
		if (Input.GetKeyDown(KeyCode.UpArrow))
			networkObject.SendRpc(RPC_MOVE, Receivers.All, Vector3.up);
	
		// Move the cube down in world space if the down arrow was pressed
		else if (Input.GetKeyDown(KeyCode.DownArrow))
			networkObject.SendRpc(RPC_MOVE, Receivers.All, Vector3.down);
	}
	
	/// <summary>
	/// Used to move the cube that this script is attached to
	/// </summary>
	/// <param name="args">
	/// [0] Vector3 The direction/distance to move this cube by
	/// </param>
	public override void Move(RpcArgs args)
	{
		// RPC calls are not made from the main thread for performance, since we
		// are interacting with Unity engine objects, we will need to make sure
		// to run the logic on the main thread
		MainThreadManager.Run(() =>
		{
			transform.position += args.GetNext<Vector3>();
		});
	}
}
```
	
As you can see from the code snippet above an RPC is called using the **networkObject.SendRPC** method. The first argument is the name of the method, the second argument is the receivers of the object which could be set to things like AllBuffered, Others, etc, and the last argument(s) are the arguments for the method. The arguments are mapped to the **object[] args** of the method in the order that they were sent in. The moment the RPC method is called it is sent on the network to be replicated to the other clients (including server if called from a client). **Note**: _In this example, it doesn't use a buffered call and it does not actually synchronize the position, so the client should be connected before the cube is moved._ **Note 2**: _Notice that we use **MainThreadManager** to run the move logic in this example, if you have not used the **MainThreadManager** before or would like more information about threading in Unity, please view [this page](threading-in-unity)._

#### Scene Setup

Now that we have done all of the network programming required for our end goal, it is time to setup our scene.

1. Create a new scene in unity
2. Create a cube which will act as the active moving cube
3. Set the position of this cube on the **x** , **y** , and **z** to **0** , **0** , **0** respectively
4. Attach our **MoveCube** script to this cube
5. Save the scene as **MoveCube**
6. Open up the build properties for Unity
7. Add the **MultiplayerMenu** scene as the first scene
8. Add your **MoveCube** scene as the second scene

#### Test

Now that we have setup our scene and everything else, it is time to test the game.

1. Open the **Build Settings**
2. Click on **Player Settings...**
1. Open the **Resolution and Presentation** section
2. Turn on **Run In Background\***
3. Go back to **Build Settings**
4. Click on **Build And Run**
5. Once the game is open, return to the Unity Editor
6. Open the **MultiplayerMenu** scene
7. Click the play button
8. Click the **Host (127.0.0.1:15937)** button on the bottom of the game view
9. Go back to the built game
10. Make sure the host ip address is set to **127.0.0.1**
11. Make sure the host port is set to **15937**
12. Click the **Connect** button
13. Select the server instance (Unity Editor) then press the up and down arrow keys
14. Select the client instance then press the up and down arrow keys

You will see the server movements replicated to the client and the client movements replicated to the server

## Troubleshooting

**Getting a null reference exception?**

The most common user errors with this part of the documentation are:

- Forgot to turn on Run in Background*
- Tried pressing the play button in the scene and not loading the Multiplayer Menu scene first
- Not setting up the multiplayer menu scene as index 0 and the demo scene as index 1
