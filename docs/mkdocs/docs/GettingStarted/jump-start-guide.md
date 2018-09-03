# Jump Start Guide

This example will go through all of the major uses of Forge Networking Remastered. You will learn the basic building blocks required to build any online multiplayer game using Forge Networking Remastered (FNR).

## Introduction

The game we will be creating for this example is very simple. We will have two or more players that can see each other as capsules and we will have a volume (trigger), in this trigger if our player activates it (triggers it) then it will spawn a sphere to the center of the map that will have a rigidbody and random x and z force. Note that the trigger will need to be destroyed when the ball is spawned in order to prevent it from spawning more than one sphere. When the sphere is collided with y the player, then the sphere is destroyed and we add a point to the player who touched the sphere.

## Very First Step

The first step before adding any networking to your game is to identify where the critical points to add networking will be. Below is a list of information we know that will need to be sent across the network in order for the game to play as expected on all connections.

* Spawn player
* Sync player positions
* Sync player names
* Destroy trigger after touch
* Spawn sphere
* Sync sphere position
* Change sphere velocity on spawn
* Add points for colliding with sphere
* Change sphere position after collision

Next we can go through each of our required network fields and determine which ones should be done with an RPC, network instantiate, network destroy, and which ones should be done by synchronizing a variable.

* **Instantiate** Spawn player
* **Variable** Sync player positions
* **RPC** Sync player names
* **Destroy** Remove trigger after touch
* **Instantiate** Spawn sphere
* **Variable** Sync sphere position
* **RPC** Change sphere velocity on spawn
* **RPC** Show the last person who got the ball
* **RPC** Change sphere position and velocity after collision

By doing this we are easily able to see what we should do next when we move onto the Network Contract Wizard (NCW).

## Setup the Game

Next, before we setup our networking logic, we will want to setup our base game and all of it's objects and prefabs (No scripting yet).

First we will create the basic world for our players to run around in.

1. Create folder:  Scenes
2. Create folder:  Scripts
3. Create folder:  Prefabs
4. Create folder:  Materials
5. Create a new scene
6. Delete the **Main Camera**
7. Save your scene as a new scene into **Scenes** folder
8. Create a material in **Materials** folder
    1. Name:  Trigger
    2. Color:  0, 255, 0, 128
    3. Set Rendering Mode to Transparent
9. Create a material in **Materials** folder
    1. Name:  GameBall
    2. Color:  0, 0, 255, 255 Do the following in the Scene we just created.

1. Create a cube:
    1. Position:  0, 0, 0
    2. Rotation:  0, 0, 0
    3. Scale:  25, 0.1, 25
    4. Name:  Floor
2. Create a cube
    1. Position:  -12.5, 5, 0
    2. Rotation:  0, 0, 0
    3. Scale:  0.1, 10, 25
    4. Name:  Left Wall
3. Create a cube
    1. Position:  12.5, 5, 0
    2. Rotation:  0, 0, 0
    3. Scale:  0.1, 10, 25
    4. Name:  Right Wall
4. Create a cube
    1. Position:  0, 5, -12.5
    2. Rotation:  0, 0, 0
    3. Scale:  25, 10, 0.1
    4. Name:  Front Wall
5. Create a cube
    1. Position:  0, 5, 12.5
    2. Rotation:  0, 0, 0
    3. Scale:  25, 10, 0.1
    4. Name:  Back Wall
6. Create a cube
    1. Position:  7.5, 5, 7.5
    2. Rotation:  0, 0, 0
    3. Scale:  10, 10, 10
    4. Name:  Start Trigger
    5. Set Material to the **Trigger** material we created
    6. Check the **Is Trigger** box in the box collider component on this object
7. Create a sphere
    1. Position:  0, 10, 0
    2. Rotation: 0, 0, 0
    3. Scale:  1, 1, 1
    4. Name:  GameBall
    5. Set Material to the **GameBall** material we created
    6. Add a **Rigidbody** component
    7. Save as a Prefab in the **Prefabs** folder
    8. Delete from scene
8. Create an empty Game Object
    1. Position:  0, 0, 0
    2. Rotation:  0, 0, 0
    3. Scale:  1, 1, 1
    4. Name:  Game Logic
9. Select the menu item **Game Object>UI>Text**
    1. The **Text** that is created in the **Hierarchy**
    2. Rename it to **Last Scored**
    3. Change the **Text** field to be empty
    4. Set the **Anchor Preset** to **Top Left** _(this is the one with the top left being highlighted in the anchor image)_ e.  Set the **Pivot X** to 0
    5. Set the **Pivot Y** to 1
    6. Set **Pos X** to 0
    7. Set **Pos Y** to 0
    8. Set **Pos Z** to 0
    9. Set **Width** to 500
11. Save the scene

Now we have completed the setup for our game, let's setup our **Build Settings**

1. Open Build Settings from **File>Build Settings** (Ctrl + Shift + B on windows)
2. Add **MultiplayerMenu** scene as the 0th scene index
3. Add your newly saved scene as the 1st index
4. Click **Player Settings...**
5. Turn on **Run In Background**

Now all that is left to do is setup our **Player** prefab and then we will be ready to jump into setting up our network.

1. In Unity select **Assets>Import Package>Characters**
2. Click the Import button
3. Open the **Standard Assets>Characters>FirstPersonCharacter>Prefabs** directory
4. Drag the **FPSController** prefab into the scene
5. Rename the prefab to **Player**
6. Add a **Mesh Filter** component
7. Add a **Mesh Renderer** component
8. Select the **Capsule** as the **Mesh** in the **MeshFilter** component
9. Open the **Materials** drop down in the **Mesh Renderer**
10. Select **Default-Material** in **Element 0**
11. Add a **Sphere Collider**
    1. Set **Y** to -0.5
    2. Set **Radius** to 0.5
12. Add another **Sphere Collider**
    1. Set **Y** to 0.5
    2. Set **Radius** to 0.5
13. Drag the **Player** GameObject from the **Hierarchy** into the **Prefabs** folder that we made in the beginning
14. Delete the **Player** Game Object from the scene 
15. Save the project

The reason we are adding 2 sphere Colliders is because (at the time of this writing) the Character Controller is not reliable for detecting collisions. You have to "rub" up on the object a bit before it is detected. By adding the sphere colliders to our character, this issue will be resolved.

Now you are ready to start setting up your network contract using the **Network Contract Wizard**.

## Network Contract Wizard

The Network Contract Wizard (NCW) is responsible for creating the blueprint of the network communication. We will use this UI to setup the basic classes, fields, and remote procedure calls needed for our network communication. To get started, open the Network Contract Wizard UI by going to **Window>Forge Networking>Network Contract Wizard** within Unity. In this window you will be presented with all the current network classes.

If you included the **Bearded Man Studios Inc** examples folder then you should see a couple pre-made options initially.

Now let's get started on making the network contract for our simple game:

1. Click the **Create** button
2. Type in **Player** into the name box
3. Click the **Add Field** button
    1. Type **position** into the text box
    2. Select the drop down
    3. Select **VECTOR3** from the options
    4. Click the **Interpolate** button and leave the value at **0.15**
4. Click the **Add RPC** button
    1. Type **UpdateName** into the text field
    2. Open the **Arguments** cascade
        1.Click the ![add-button](https://raw.githubusercontent.com/BeardedManStudios/ForgeNetworkingRemastered/develop/docs/mkdocs/docs/images/add.png "Add Button") button
        2. Type **newName** into the text field
        3. Click the drop down iv.  Select **STRING** from the drop down
5. Click **Save &amp; Compile**

What this will do is create and modify some classes in the **Generated** folder of your project. Before we continue to add the other network contracts, let's setup our **Player** class.

1. Open the **Scripts** folder
2. Create a new C# script named **Player**
3. Open the **Prefabs** folder and select the **Player** prefab
4. Click **Add Component**
5. Add the **Player** script to the prefab
6. Select the **Scripts** folder
7. Open the **Player.cs** script you just created 
8. Insert the following code

### Player.cs**
```csharp
// We use this namespace as it is where our PlayerBehavior was generated using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

// We extend PlayerBehavior which extends NetworkBehavior which extends MonoBehaviour
public class Player : PlayerBehavior
{
	private string[] nameParts = new string[] { "crazy", "cat", "dog", "homie", "bobble", "mr", "ms", "mrs", "castle", "flip", "flop" };

	public string Name { get; private set; }

	protected override void NetworkStart()
	{
		base.NetworkStart();

		if (!networkObject.IsOwner)
		{
			// Don't render through a camera that is not ours
			// Don't listen to audio through a listener that is not ours
			transform.GetChild(0).gameObject.SetActive(false);

			// Don't accept inputs from objects that are not ours
			GetComponent<FirstPersonController>().enabled = false;

			// There is no reason to try and simulate physics since the position is
			// being sent across the network anyway
			Destroy(GetComponent<Rigidbody>());
		}

		// Assign the name when this object is setup on the network
		ChangeName();
	}

	public void ChangeName()
	{
		// Only the owning client of this object can assign the name
		if (!networkObject.IsOwner)
		return;
		// Get a random index for the first name
		int first = Random.Range(0, nameParts.Length - 1);
		// Get a random index for the last name
		int last = Random.Range(0, nameParts.Length - 1);

		// Assign the name to the random selection
		Name = nameParts[first] + " " + nameParts[last];

		// Send an RPC to let everyone know what the name is for this player
		// We use "AllBuffered" so that if people come late they will get the
		// latest name for this object
		// We pass in "Name" for the args because we have 1 argument that is to
		// be a string as it is set in the NCW
		networkObject.SendRpc(RPC_UPDATE_NAME, Receivers.AllBuffered, Name);
	}

	// Default Unity update method
	private void Update()
	{
		// Check to see if we are the owner of this player
		if (!networkObject.IsOwner)
		{
			// If we are not the owner then we set the position to the
			// position that is syndicated across the network for this player
			transform.position = networkObject.position;
			return;
		}

		// When our position changes the networkObject.position will detect the
		// change based on this assignment automatically, this data will then be
		// syndicated across the network on the next update pass for this networkObject
		networkObject.position = transform.position;
	}

	// Override the abstract RPC method that we made in the NCW
	public override void UpdateName(RpcArgs args)
	{
		// Since there is only 1 argument and it is a string we can safely
		// cast the first argument to a string knowing that it is going to
		// be the name for this player
		Name = args.GetNext<string>();
	}
}
```

This is all the code we need to allow for all of the connections to see the movement of the players. The next thing we need is to be able to actually instantiate our **Player** prefab since it will not be in the scene at the start of the game. To do this let's open the **NCW** window again.

1. Click the **Create** button
2. Type in **GameLogic** in the name box
3. Click the **Add RPC** button
    1. Type **PlayerScored** into the text field
    2. Open the **Arguments** cascade
        1. Click the ![add-button](https://raw.githubusercontent.com/BeardedManStudios/ForgeNetworkingRemastered/develop/docs/mkdocs/docs/images/add.png "Add Button") button
        2. Type **playerName** into the text field
        3. Click the drop down iv.  Select **STRING** from the drop down
4. Click **Save &amp; Compile**

What this will do is create and modify some classes in the **Generated** folder of your project. Before we continue to add the other network contracts, let's setup our **GameLogic** class.

1. Open the **Scripts** folder
2. Create a new C# script named **GameLogic**
3. Select the **Game Logic** Game Object from the **Hierarchy** of our previously saved scene
4. Click **Add Component**
5. Add the **GameLogic** script to the Game Object
6. Select the **Scripts** folder
7. Open the **GameLogic.cs** script you just created
8. Insert the following code

### GameManager.cs
```csharp
// We use this namespace as it is where our GameLogicBehavior was generated
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.UI;

// We extend GameLogicBehavior which extends NetworkBehavior which extends MonoBehaviour
public class GameLogic : GameLogicBehavior
{
	public Text scoreLabel;
	private void Start()
	{
		// This will be called on every client, so each client will essentially instantiate
		// their own player on the network. We also pass in the position we want them to spawn at
		NetworkManager.Instance.InstantiatePlayer(position: new Vector3(0, 5, 0));
	}

	// Override the abstract RPC method that we made in the NCW
	public override void PlayerScored(RpcArgs args)
	{
		// Since there is only 1 argument and it is a string we can safely
		// cast the first argument to a string knowing that it is going to
		// be the name for the scoring player
		string playerName = args.GetNext<string>();

		// Update the UI to show the last player that scored
		scoreLabel.text = "Last player to score was: " + playerName;
	}
}
```

Now we are able to not only spawn the player, but we are also able to print out the last player that scored to the screen. Talking about scoring, we possibly want to get the ball rolling (if you would excuse the expression) and instantiate the **ball** and serialize it's position to all the clients. However just before that, lets fill out our scoreLabel object on our **Game Logic** Game Object.

1. Select the **Game Logic** from the **hierarchy**
2. Drag the **Last Scored** UI Game Object from the **hierarchy** to the **Score Label** field on the **GameLogic** script

Okay, now we are actually ready to make the ball now! So let's start by opening up the **NCW** window once again.

1. Click the **Create** button
2. Type in **GameBall** into the name box
3. Click the **Add Field** button
    1. Type **position** into the text box
    2. Select the drop down
    3. Select **VECTOR3** from the options
    4. Click the **Interpolate** button and leave the value at **0.15**
4. Click **Save &amp; Compile**

What this will do is create and modify some classes in the **Generated** folder of your project. Before we continue to add the other network contracts, let's setup our **GameBall** class.

1. Open the **Scripts** folder
2. Create a new C# script named **GameBall**
3. Open the **Prefabs** folder and select the **GameBall** prefab
4. Click **Add Component**
5. Add the **GameBall** script to the prefab
6. Select the **Scripts** folder
7. Open the **GameBall.cs** script you just created
8. Insert the following code

### GameBall.cs
```csharp
// We use this namespace as it is where our BallBehavior was generated
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

// We extend BallBehavior which extends NetworkBehavior which extends MonoBehaviour
public class GameBall : GameBallBehavior
{
	private Rigidbody rigidbodyRef;
	private GameLogic gameLogic;

	private void Awake()
	{
		rigidbodyRef = GetComponent<Rigidbody>();
		gameLogic = FindObjectOfType<GameLogic>();
	}

	// Default Unity update method
	private void Update()
	{
		// Check to see if we are the owner of this ball
		if (!networkObject.IsOwner)
		{
			// If we are not the owner then we set the position to the
			// position that is syndicated across the network for this ball
			transform.position = networkObject.position;
			return;
		}

		// When our position changes the networkObject.position will detect the
		// change based on this assignment automatically, this data will then be
		// syndicated across the network on the next update pass for this networkObject
		networkObject.position = transform.position;
	}

	public void Reset()
	{
		// Move the ball to 0, 10, 0
		transform.position = Vector3.up * 10;

		// Reset the velocity for this object to zero
		rigidbodyRef.velocity = Vector3.zero;

		// Create a random force to apply to this object between 300 to 500 or -300 to -500
		Vector3 force = new Vector3(0, 0, 0);
		force.x = Random.Range(300, 500);
		force.z = Random.Range(300, 500);

		// Randomly invert along the number line by 50%
		if (Random.value < 0.5f)
			force.x *= -1;

		if (Random.value < 0.5f)
			force.z *= -1;

		// Add the random force to the ball
		rigidbodyRef.AddForce(force);
	}

	private void OnCollisionEnter(Collision c)
	{
		// We are making this authoritative by only
		// allowing the server to call it
		if (!networkObject.IsServer)
			return;

		// Only move if a player touched the ball
		if (c.GetComponent<Player>() == null)
			return;

		// Call an RPC on the Game Logic to print the player's name as the last
		// player to touch the ball
		gameLogic.networkObject.SendRpc(GameLogicBehavior.RPC_PLAYER_SCORED, Receivers.All,

		c.GetComponent<Player>().Name);

		// Reset the ball
		Reset();
	}
}
```

With the ball complete, we are finally ready to move onto the last script that we will need to create, the behavior for the trigger that starts the whole game!

1. Open the **Scripts** folder
2. Create a new C# script named **GameTrigger**
3. Select the **Start Trigger** Game Object from the **Hierarchy** of our previously saved scene
4. Click **Add Component**
5. Add the **GameTrigger** script to the Game Object
6. Select the **Scripts** folder
7. Open the **GameTrigger.cs** script you just created
8. Insert the following code

### GameTrigger.cs
```csharp
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class GameTrigger : MonoBehaviour
{
	private bool started;

	private void Update()
	{
		// If the game started we will remove this trigger from the scene
		if (FindObjectOfType<GameBall>() != null)
			Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider c)
	{
		// Since we added 2 sphere colliders to the player, we need to
		// make sure to only trigger this 1 time
		if (started)
			return;

		// Only allow the server player to start the game so that the
		// server is the owner of the ball, otherwise if a client is the
		// owner of the ball, if they disconnect, the ball will be destroyed
		if (!NetworkManager.Instance.IsServer)
			return;

		Player player = c.GetComponent<Player>();

		if (player == null)
			return;

		started = true;

		// We need to create the ball on the network
		GameBall ball = NetworkManager.Instance.InstantiateGameBall() as GameBall;

		// Reset the ball position and give it a random velocity
		ball.Reset();

		// We no longer need this trigger, the game has started
		Destroy(gameObject);
	}
}
```

**Finalization of our game!**

Now that we have finished all of the network code, we need to tell Forge what prefabs are suppose to be instantiated when we make those **NetworkManager.Instance.Instantiat...** calls.

1. Open **Bearded Man Studios Inc>Prefabs**
2. Select the prefab named **NetworkManager**
3. Locate the various empty fields in the **NetworkManager** component that is attached to this prefab
4. In the **GameBall Network Object** array field, put the **GameBall** prefab from the **Prefabs** folder as the **Element 0** (0th index)
5. In the **Player Network Object** array field, put the **Player** prefab from the **Prefabs** folder as the **Element 0** (0th index)

## Congratulations

You have completed the steps for this tutorial. All that is left is to build, run and test it out. To do this just build the project as you normally would do within the Unity Editor.

1. Build the project
2. Run 2 instances of the project
3. Select **Host** in one instance
4. Click **Connect** in the second instance
5. You may be prompted to allow access to the application on the firewall, which you will need to accept

## Troubleshooting

**Getting a null reference exception?**

The most common user errors with this part of the documentation are:

- Forgot to turn on Run in Background*
- Tried pressing the play button in the scene and not loading the Multiplayer Menu scene first
- Not setting up the multiplayer menu scene as index 0 and the demo scene as index 1
