using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

/* NOTICE
 * The server is the source of truth for the network, so when you run this
 * sample, keep in mind that proximity doesn't work with the server player.
 * The best way to use this sample is to build out 2 clients and then run
 * them and test the proximity between them (not the server). You will notice
 * that the server is always up to date, which is the desired behavior
 */

public class ExampleProximityPlayer : ExampleProximityPlayerBehavior
{
	/// <summary>
	/// The speed that the object moves by when the horizontal or vertical axis has a value
	/// </summary>
	public float speed = 5.0f;

	/// <summary>
	/// The reference to the material on this object
	/// </summary>
	public Material matRef = null;

	/// <summary>
	/// The distance allowed for the proximity between objects in Unity unit space
	/// </summary>
	public float proximityDistance = 1.0f;

	public void Start()
	{
		matRef = GetComponent<Renderer>().material;
	}

	protected override void NetworkStart()
	{
		base.NetworkStart();

		// We assign this so that when messages are sent via proximity, the server knows
		// who to send the message to based on their players distance
		networkObject.Networker.ProximityDistance = proximityDistance;
	}

	public void Update()
	{
		// If the network object is not setup yet we do nothing with this object
		if (networkObject == null)
			return;

		// If we are not the owner of this object we move to the position provided on the network
		if (!networkObject.IsOwner)
		{
			transform.position = networkObject.position;

			// The server will be authoritative in updating the player's unit vector
			if (networkObject.Networker.IsServer)
				UpdatePlayerProximityOnServer();

			return;
		}

		// Move the object with the horizontal and vertical inputs along the x and y respectively
		transform.position += new Vector3(
			Input.GetAxis("Horizontal") * speed * Time.deltaTime,
			Input.GetAxis("Vertical") * speed * Time.deltaTime,
			0
		);

		// Send the new position across the network if there are any changes to it
		networkObject.position = transform.position;

		// If we press the spacebar we will pick a new random color to assign to the object
		// Notice that the receivers are proximity based
		if (Input.GetKeyDown(KeyCode.Space))
			networkObject.SendRpc(RPC_SEND_COLOR, Receivers.AllProximity, new Color(Random.value, Random.value, Random.value));

		if (networkObject.Networker.IsServer)
			UpdatePlayerProximityOnServer();
	}

	/// <summary>
	/// Used to check and see if this object is in a new proximity unit
	/// and if so it will update it's unit on the server
	/// </summary>
	private void UpdatePlayerProximityOnServer()
	{
		// The proximity location is a Vector with 3 components (x, y, and z) that identify the location
		// that the owning player for this object should be treated at on the network. This is NOT per-object
		// it is per player, which means you normally want to assign the ProximityLocation of a player to the
		// object that identifies them. If you do not identify players by objects, then you should create a
		// concept of the player location dynamically through code OR not use proximity based messages

		// Set the player's location to the location of this object if they are the owner. Remember that this
		// is only done on the server
		networkObject.Owner.ProximityLocation = new Vector(
			transform.position.x,
			transform.position.y,
			transform.position.z
		);
	}

	/// <summary>
	/// The RPC for setting the color of this object
	/// </summary>
	/// <param name="args">The color is the only argument sent</param>
	public override void SendColor(RpcArgs args)
	{
		// Assign the object color to the one provided on the network, this
		// is done over a proximity based RPC so only players within the proximity
		// will get this RPC
		matRef.color = args.GetNext<Color>();
	}
}