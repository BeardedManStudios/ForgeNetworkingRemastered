# Basic Instantiation Example

_Note: We assume that you have gone through the_ _Basic Moving Cube Example_ _and the_ _Basic RPC Example_ _before going through this example._

**This example is not a continuation of other examples and should be treated as if it were done in a new project.**

In this example we are going to create a simple cube to act as the player object. The "player" should be able to move their individual cube using the **horizontal** and **vertical** axis input. For this we will create a scene without any cubes in it, then when a connection is made or the server is created, we will spawn a cube "player" for the newly connected client or created server.

## Planning Network Code

So one of the first things we want to think about is our **Network Contract**. This is basically a fancy word for how we design/setup network communication. It is helpful to imagine our final result to our application so that we can properly design for it. In this application when a client connects it will create a player cube for itself, it will also see all of the other connected cubes and their movements:

1. Player creates a cube for itself when connected
2. Synchronize the position and rotation of the player cube to all the clients and server
3. The transformations should be smooth so we need to interpolate them

## Network Contract Wizard

Now that we know that we need to sync the **position** and **rotation** of a cube, we can design our network contract for that object. We will first open the **Network Contract Wizard** which is a UI provided by the Bearded Man Studios team to make it easy to design your **network contracts** in a easy way. To open this menu, go into Unity and select "Window->Forge Networking->Network Contract Wizard".

![opening-the-ncw](https://raw.githubusercontent.com/BeardedManStudios/ForgeNetworkingRemastered/develop/docs/mkdocs/docs/images/opening-ncw.jpg "How to open the NCW")

Once you have opened this editor you will be presented with a list of all the Network Objects currently available, to learn more about this please see the document on the [Network Contract Wizard](network-contract-wizard-ncw) as we will just be going over how to create a network object through the contract wizard. To start, click on the "Create" button near the top and you will be presented with the create UI. In here, we have 3 major fields of interest, the **Name** fi elds, the **Fields** field, and the **Remote Procedure Calls** field.

1. The **Name** field is where we create the name for our Network Object and behavior, this is a friendly name that should be written in "Pascal case" to follow the C# convention since it is going to be a part of the class names that are generated.
2. The **Fields** section shows all of the various fields that our network should be aware of. In this case we are going to want to make a **positi on** and **rotation** field which are **Vector3** and **Quaternion** respectively. _Note, you can name these fields whatever you like, these are just friendly variable names for you (and your team) to know what they are for when used_
3. The **Remote Procedure Calls** field is where you will design any Remote Procedure Call (RPC) function signatures. We are not going to go over this field in this tutorial as we do not need it for the goal we are attempting to accomplish.

Lets begin by naming our Network Object:

1. Lets set the name for our Network object to " **PlayerCube**" (without quotes)
2. Click the **Add Field** button
3. Name the new field **position**
4. Set the type to **Vector3**
5. Click the **Interpolate** button
6. Set the interpolate time (the text field that pops up after clicking the **Interpolate** button) as **0.15**
7. Click the **Add Field** button
8. Name the new field **rotation**
9. Set the type to **Quaternion**
10. Click the **Interpolate** button
11. Set the interpolate time (the text field that pops up after clicking the **Interpolate** button) as **0.15** 12.  Click the **Save & Compile** button

![player-cube](https://raw.githubusercontent.com/BeardedManStudios/ForgeNetworkingRemastered/develop/docs/mkdocs/docs/images/player-cube.jpg "Player Cube")

## Extending Generated Classes

When we use the **Network Contract Wizard (NCW)** we are actually generating a lot of network code based on what has been input into the editor fields, this actually cuts out a lot of work that you would have to do by hand. There is one class in particular that we want to extend from, this class name will be " **PlayerCubeBehavior**". The naming convention for this generated class is \_\_\_\_\_Behavior where "\_\_\_\_\_" is the name we typed into the NCW. Lets now create a C# file in Unity and write our basic game logic, we will name this file " **PlayerCube**".

1. Open the newly created C# file
2. Add **using BeardedManStudios.Forge.Networking.Generated;** to the using statements
3. Derive the class from **PlayerCubeBehavior**
4. Write the rest of the logic for the cube as seen below

### PlayerCube
```csharp
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;

public class PlayerCube : PlayerCubeBehavior
{
	/// <summary>
	/// The speed that this cube should move by when there are axis inputs
	/// </summary>
	public float speed = 5.0f;

	private void Update()
	{
		// If this is not owned by the current network client then it needs to
		// assign it to the position and rotation specified
		if (!networkObject.IsOwner)
		{
			// Assign the position of this cube to the position sent on the network
			transform.position = networkObject.position;

			// Assign the rotation of this cube to the rotation sent on the network
			transform.rotation = networkObject.rotation;

			// Stop the function here and don't run any more code in this function
			return;
		}

		// Get the movement based on the axis input values
		Vector3 translation = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

		// Scale the speed to normalize for processors
		translation *= speed * Time.deltaTime;

		// Move the object by the given translation
		transform.position += translation;

		// Just a random rotation on all axis
		transform.Rotate(new Vector3(speed, speed, speed) * 0.25f);

		// Since we are the owner, tell the network the updated position
		networkObject.position = transform.position;

		// Since we are the owner, tell the network the updated rotation
		networkObject.rotation = transform.rotation;

		// Note: Forge Networking takes care of only sending the delta, so there
		// is no need for you to do that manually
    }
}
```

## Setting up prefab

Since we are going to instantiate this object when a player connects, we will need a prefab that identifies this object. To quickly create a prefab that has a cube we will use the standard Unity process.

1. **Right click** on the **Hierarchy**
2. Hover our mouse over **3D Object**
3. Click on **Cube** in the following context menu

Now that we have created a cube we need to set it up with the class **PlayerCube** class we just created

1. Select the cube
2. Click on the **Add Component** button in the **Inspector**
3. Type in **PlayerCube** into the search
4. Click the **PlayerCube** script to attach it

Now that we have setup our player object it is time for us to create the prefab that will be instantiated when clients connect.

1. Select the cube in the **Hierarchy**
2. Drag the cube into the **Project** area to create the prefab
3. Rename this prefab **PlayerCube** , this step is not required, just helps the example stay cohesive

With this we are prepared to setup our **NetworkManager** to support the new instantiation of the object.

## Attaching the PlayerCube prefab for instantiation

If you search the project directory you will find a prefab named **NetworkManager**. This is a default prefab we have created for you to get started. You **can** make your own prefab or alter this one if you wish to extend behavior. Now we will go through the process of attaching our created **PlayerCube** prefab to this **NetworkManager**

1. Select the **NetworkManager** prefab in the project
2. Locate the field named **Player Cube Network Object**
3. You will notice that this is an array, set the **Size** to 1
4. Click the radial (circle) on the right of the **Element 0** input field
5. Locate and select the **PlayerCube** prefab

## Setup Instantiation &amp; Scene

Now that we have setup our **NetworkManager** we are ready to make our instantiate code. To do this we will create a script that will instantiate our player

1. Open up a new scene
2. Create a new C# script named **GameLogic**
3. Open the newly created C# file
4. Add **using BeardedManStudios.Forge.Networking.Unity;** to the using statements
5. Write the rest of the logic for instantiating the cube (seen below)
6. Attach the newly created script to the **Main Camera** object in the scene (this is just to have the script in the scene, no other particular reason)
7. Save the scene as **GameScene**
8. Add the **MultiplayerMenu** scene as the first scene
9. Add your **GameScene** scene as the second scene

### GameLogic
```csharp
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;
public class GameLogic : MonoBehaviour
{
	private void Start()
	{
		NetworkManager.Instance.InstantiatePlayerCube();
	}
}
```

## Test

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
13. Select the server game instance (Unity Editor)

Now if you move around the cube in the editor, you will see the movements replicated to the client(s). If you move the cube around in the client(s) you will see the cube moving on the server. Our code has the cube constantly rotating so you will see them doing that as well.

## Troubleshooting

**Getting a null reference exception?**

The most common user errors with this part of the documentation are:

- Forgot to turn on **Run in Background\***
- Tried pressing the play button in the scene and not loading the **MultiplayerMenu** scene first
- Not setting up the **MultiplayerMenu** scene as index 0 and the demo scene as index 1
